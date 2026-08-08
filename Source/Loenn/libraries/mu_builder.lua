return function(mu)
  --[[
    entity/trigger/etc table builder.

    calling mu.builder (or its wrappers mu.entity / mu.trigger) produces a special
    table that can be used to construct placements, fieldOrder/Information, and
    lang entries.
    it is standard to name builders `self`, like this:

    > local self = mu.entity {
    >   "Example",          -- required; corresponds to [CustomEntity("Microlith57Misc/Example")]
    >   name = "Example",   -- optional; used as default localised placement name
    >   desc = "An example" -- optional; used as default placement tooltip
    > }
    >
    > self.something = 1.0
    > self.something.desc = "A number that does something."
    > self.another = "another"
    > self.another.desc = "Does something else."
    > self.another:nonempty()
    >
    > return self {
    >   {}, -- default placement (inherits name, desc, data)
    >   {
    >     "differentPlacement",
    >     name = "Example (Different)",
    >     data = { another = "different" }
    >   },
    > }
  ]]

  local celeste_enums = require("consts.celeste_enums") -- see we're countercultural here. snake_case instead of camelCase

  function mu.validate_nonempty(s) return s ~= "" end

  ---@class _FlagOrExpr
  ---@field [1|"bool"] string?
  ---@field set string?
  ---@field unset string?
  ---@field name string?
  ---@field default string?
  ---@field desc string?
  ---@field invert string?
  ---@field invertFlag boolean?
  ---@field defaultInvert boolean?

  ---@class _Builder_UseRawDeltaTime
  ---@field [1|"default"] boolean?
  ---@field name string?
  ---@field desc string?

  ---@alias _Builder_PositionMode {name: string?, desc: string?}
  ---@alias _Builder_AngleFormat {name: string?, desc: string?}

  ---@class _Builder_Placement
  ---@field [1] string?
  ---@field name string?
  ---@field desc string?
  ---@field data {[string]: any}?

  ---@class _Builder_Call
  ---@field [integer] _Builder_Placement
  ---@field [string] any

  ---@class Builder
  ---@field _lang    fun(self: Builder, key: string): Builder
  ---@field _xy      fun(self: Builder): Builder
  ---@field _rect    fun(self: Builder): Builder
  ---@field _depth   fun(self: Builder, depth: integer|false?): Builder
  ---@field _tags    fun(self: Builder, tags: string[]|false?): Builder
  ---@field _assoc   fun(self: Builder, tbl: {[string]: boolean}): Builder
  ---@field _extra   fun(self: Builder, tbl: {[string]: any}): Builder
  ---@field _texture fun(self: Builder, tex: string|table?): Builder
  ---@field _flag_or_expr   fun(self: Builder, tbl: _FlagOrExpr): Builder
  ---@field _raw_delta_time fun(self: Builder, tbl: _Builder_UseRawDeltaTime?): Builder
  ---@field _position_mode  fun(self: Builder, tbl: _Builder_PositionMode?): Builder
  ---@field _angle_format   fun(self: Builder, tbl: _Builder_AngleFormat?): Builder
  ---@field _interleave     fun(self: Builder, things: Fmt[], fields: string[])
  ---@field _placement fun(self: Builder, tbl: _Builder_Placement?): Builder
  ---@operator call(_Builder_Call): table
  ---
  ---@field package name        string
  ---@field package _base_name  string
  ---@field package _name       string?
  ---@field package _desc       string?
  ---@field package _fields     {[string]: any}
  ---@field package _order      string[]
  ---@field package _order_set  {[string]: true?}
  ---@field package _info       {[string]: table?}
  ---@field package _assoc_mods {[string]: boolean?}
  ---@field package _data       {[string]: any}
  ---@field package _placements table[]
  ---
  ---@field [string] Field
  local Builder = {}

  ---@class Field
  ---@field package _builder Builder
  ---@field package _field string
  ---@operator call(any): Field
  local Field = {}

  function Builder:__index(key)
    if Builder[key] then return Builder[key] end
    local field = {
      _builder = self,
      _field = key
    }
    setmetatable(field, Field)
    return field
  end
  function Builder:__newindex(key, val)
    if Builder[key] then
      Builder[key](val)
      return self
    end
    if not self._order_set[key] then
      self._order_set[key] = true
      table.insert(self._order, key)
    end
    self._fields[key] = val
  end
  function Builder:_lang(key)
    if mu.preprocess then
      rawset(self, "_lang", mu.preprocess.lang[key][self.name])
    end
    return self
  end
  function Builder:_xy()
    self.x = nil self.y = nil
    return self
  end
  function Builder:_rect()
    self.x = nil self.y = nil self.width = nil self.height = nil
    return self
  end
  function Builder:_depth(depth)
    if depth == false then return self end
    self.depth = depth
    self.depth:optional():int()
    return self
  end
  function Builder:_tags(tags)
    -- TODO custom field type
    if tags == false then return self end
    tags = tags or {"PauseUpdate", "FrozenUpdate", "TransitionUpdate"}

    ---@param v string
    local function warningValidator(v)
      v = v:gsub("^%s*", ""):gsub("%s+$", "")
      for _, t in ipairs(tags) do
        if v == t then return true end
      end
      return false
    end

    ---@param vs string
    local function valueTransformer(vs)
      local t = {} ---@type string[]
      local tags_set = {} ---@type {[string]: true?}
      local vt = string.split(vs, ",")()
      table.sort(vt)
      for _, v in ipairs(vt) do
        v = v:gsub("^%s*", ""):gsub("%s+$", "")
        if not tags_set[v] then
          table.insert(t, v)
          tags_set[v] = true
        end
      end
      return table.concat(t, ",")
    end

    self.tags ""
      :info {
        fieldType = "list",
        elementSeparator = ",",
        elementOptions = {
          options = tags,
          warningValidator = warningValidator
        },
        valueTransformer = valueTransformer
      }
      :desc "Additional tags for this entity."

    return self
  end
  function Builder:_assoc(tbl)
    for k, v in pairs(tbl) do
      self._assoc_mods[k] = v
    end
    return self
  end
  function Builder:_extra(tbl)
    for k, v in pairs(tbl) do
      self._data[k] = v
    end
    return self
  end
  function Builder:_texture(tex)
    local dst
    if type(tex) == "table" then
      tex[1] = tex[1] or self._base_name
      dst = mu.texture(tex)
    elseif type(tex) == "string" then
      dst = mu.texture {tex}
    elseif tex == nil then
      dst = mu.texture {self._base_name}
    else
      error("invalid texture type " .. type(tex))
    end
    self._data.texture = dst:gsub("^Graphics/Atlases/%a+/", "")
    return self
  end
  function Builder:_flag_or_expr(tbl)
    tbl.bool = tbl.bool or tbl[1] or "flag"
    local expr = tbl.bool == "expression"
    tbl.set = tbl.set or (expr and "truthy" or "set")
    tbl.unset = tbl.unset or (expr and "falsy" or "unset")
    tbl.desc = tbl.desc or "If present, only {action} when this {bool} is {set}."
    mu.fmt(tbl)

    local name = tbl.name or tbl.bool
    local invert = tbl.invert or "invertFlag"
    local defaultInvert = false
    if tbl.defaultInvert ~= nil then defaultInvert = tbl.defaultInvert end

    self[name]
      :default(tbl.default or "")
      :desc(tbl(tbl.desc))
    if not expr and (tbl.invertFlag ~= false) then self[invert]:default(defaultInvert) end

    self:_assoc {expr = expr}

    return self
  end
  function Builder:_raw_delta_time(tbl)
    tbl = tbl or {}
    tbl.name = tbl.name or "useRawDeltaTime"
    tbl.desc = tbl.desc or "If true, use real time (unaffected by slowed/sped up time); otherwise use normal game time."

    tbl.default = tbl.default
    if tbl.default == nil then tbl.default = tbl[1] end
    if tbl.default == nil then tbl.default = false end

    self[tbl.name](tbl.default):desc(tbl.desc)
    return self
  end
  function Builder:_position_mode(tbl)
    tbl = tbl or {}
    tbl[1] = tbl[1] or "direction"
    tbl.name = tbl.name or "Direction"
    tbl.desc = tbl.desc or "Determines how the player's position within the trigger will {action}."
    mu.fmt(tbl)

    local f = self[tbl[1]]
    if not f:has_default() then f:default "LeftToRight" end
    f:list(celeste_enums.trigger_position_modes)
      :name(tbl.name)
      :desc(tbl(tbl.desc))

    return self
  end
  function Builder:_angle_format(tbl)
    tbl = tbl or {}
    tbl.name = tbl.name or "format"
    tbl.desc = tbl.desc or "Format to use for the angle."

    tbl.desc = tbl.desc .. [[

      \b
      ZeroToOne: Fraction of a turn around a full circle, in [0.0, 1.0).
      Radians: Angle in radians, in [0.0, 2*pi).
      Radians: Angle in degrees, in [0.0, 360.0).
    ]]

    local f = self[tbl.name]
    if not f:has_default() then f:default "ZeroToOne" end
    f:name "Angle Format"
      :desc(tbl.desc)
      :list {"ZeroToOne", "Radians", "Degrees"}

    return self
  end
  function Builder:_interleave(things, fields)
    for _, f in ipairs(fields) do
      for _, t in ipairs(things) do
        local field = t(f)
        self[field]()
      end
    end
  end

  Field.__index = Field
  function Field:__newindex(k, v)
    local f = Field[k]
    if not f then error(("attempt to set %s on field %s"):format(k, self._field)) end
    f(self, v)
  end
  function Field:default(default)
    self._builder[self._field] = default
    return self
  end
  Field.__call = Field.default
  function Field:has_default()
    return self._builder._fields[self._field] ~= nil
  end
  ---@param name string
  function Field:name(name)
    if mu.preprocess then
      self._builder._lang.attributes.name[self._field] = name
    end
    return self
  end
  ---@param desc string
  function Field:desc(desc)
    if mu.preprocess then
      self._builder._lang.attributes.description[self._field] = desc
    end
    return self
  end
  ---@param info table
  function Field:info(info)
    local i = self._builder._info[self._field] or {}
    for k, v in pairs(info) do
      i[k] = v
    end
    self._builder._info[self._field] = i
    return self
  end
  ---@param val function
  function Field:validator(val)
    return self:info{validator = val}
  end
  function Field:nonempty()
    -- todo: number allowEmpty
    return self:validator(mu.validate_nonempty)
  end
  function Field:optional()
    return self:info {allowEmpty = true}
  end
  function Field:int()
    return self:info {fieldType = "integer"}
  end
  ---@param min number
  ---@param max number
  function Field:range(min, max)
    return self:info {minimumValue = min, maximumValue = max}
  end
  ---@param tbl {[integer]: string, editable: boolean?}
  function Field:list(tbl)
    local options = {}
    for i, o in ipairs(tbl) do options[i] = o end

    local editable = tbl.editable or false

    return self:info {
      options = options,
      editable = editable,
    }
  end

  ---@param name string
  local function default_placement_name(name)
    return name
      :gsub("^" .. mu.modname .. "/", "")
      :gsub("_.*", "")
      :gsub("^.", string.lower)
  end

  ---@param name string
  local function group_name(name)
    return name
      :gsub("_.*", "")
  end

  local allowed_undoc = {
    x = true, y = true,
    width = true, height = true,
    depth = true,
  }

  function Builder:_placement(tbl)
    tbl = tbl or {}
    local name = tbl[1] or default_placement_name(self.name)
    if mu.preprocess then
      self._lang.placements.name[name] = tbl.name or rawget(self, "_name")
      self._lang.placements.description[name] = tbl.desc or rawget(self, "_desc")
    end
    local data = table.shallowcopy(self._fields)
    for k, v in pairs(tbl.data or {}) do
      data[k] = v
    end
    local res = {
      name = name,
      data = data,
      ext_group = group_name(self.name)
    }
    table.insert(self._placements, res)
    return self
  end
  function Builder:__call(tbl)
    tbl = tbl or {}

    if tbl[1] then
      for _, p in ipairs(tbl) do
        self:_placement(p)
      end
    elseif #self._placements == 0 then
      self:_placement()
    end

    local result = {
      name = self.name,
      placements = self._placements,
      fieldInformation = self._info,
      fieldOrder = self._order,
      associatedMods = mu.assoc(self._assoc_mods)
    }

    if mu.preprocess then
      for _, f in ipairs(self._order) do
        local desc = self._lang.attributes.description[f]
        local is_documented = type(desc) == "string" and not desc:match("^%s*$")
        local allowed_undocumented = allowed_undoc[f] or f:match("^invert")

        if not is_documented and not allowed_undocumented then
          print(("plugin %s: undocumented field %s"):format(self.name, f))
        end
      end
    end

    for k, v in pairs(self._data) do result[k] = v end
    for k, v in pairs(tbl) do
      if type(k) ~= "number" then
        result[k] = v
      end
    end

    return result
  end

  ---@param name string
  local function prepare_name(name)
    if not name:match("^" .. mu.modname .. "/")  then
      name = mu.modname .. "/" .. name
    end
    return name
  end

  ---@param name string
  local function unprepare_name(name)
    return name:gsub("^" .. mu.modname .. "/", ""):gsub("_.*", "")
  end

  ---@param tbl {
  ---  [1]: string,
  ---  name: string?,
  ---  desc: string?,
  ---}
  ---@return Builder
  function mu.builder(tbl)
    local name = prepare_name(tbl[1])
    local base_name = unprepare_name(name)
    local builder = {
      name = name,
      _base_name = base_name,
      _name = tbl.name,
      _desc = tbl.desc,

      _fields = {},
      _order = {},
      _order_set = {},
      _info = {},
      _assoc_mods = {},
      _data = {},
      _placements = {},
    }
    setmetatable(builder, Builder)
    return builder
  end
  ---@param tbl {
  ---  [1]: string,
  ---  name: string?,
  ---  desc: string?,
  ---  depth: false | integer?,
  ---  tags: false | string[]?,
  ---}
  function mu.entity(tbl)
    return mu.builder(tbl)
      :_lang("entities")
      :_xy()
      :_depth(tbl.depth)
      :_tags(tbl.tags)
  end
  ---@param tbl {
  ---  [1]: string,
  ---  name: string?,
  ---  desc: string?,
  ---  tags: false | table?,
  ---  texture: string | table?,
  ---}
  function mu.controller(tbl)
    ---@diagnostic disable-next-line: param-type-mismatch
    local self = mu.entity(tbl)
    self:_extra {depth = -1000000}

    tbl.texture = (tbl.texture ~= nil) and tbl.texture or {only_editor = true}
    if type(tbl.texture) == "string" then tbl.texture = {tbl.texture, only_editor = true} end
    if tbl.texture then
      self:_texture(tbl.texture)
    end

    return self
  end
  ---@param tbl {
  ---  [1]: string,
  ---  name: string?,
  ---  desc: string?,
  ---  tags: false | table?,
  ---}
  function mu.trigger(tbl)
    return mu.builder(tbl)
      :_lang("triggers")
      :_rect()
      :_tags(tbl.tags)
  end
end
