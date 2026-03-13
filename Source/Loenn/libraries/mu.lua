local mu = (mu and mu.preprocess and mu) or {}

--[[
  miscellaneous utilities.

  this is part of the preprocessing system; see the 'preprocess' directory (in
  the mod's source, not a .zip) for that entrypoint.
  the key idea is that each plugin file can be evaluated either at runtime like
  normal, or at preprocess time (before building the zip); the latter being used
  to automatically generate the lang file (and potentially extract other
  metadata).
  this is especially useful since this helper makes extensive use of
  parametrised entities; so being able to define lang entries in code is very
  useful.

  as with the rest of this mod, this is MIT licensed; if you would like to adapt
  this to work with your helper, please get in touch.
]]

mu.modname = "Microlith57Misc"

function mu.validate_nonempty(s) return s ~= "" end

---

--[[
  associatedMods builder.

  usage:
  > mu.assoc {}             --> {"Microlith57MiscellaneousMechanics"}
  > mu.assoc {expr = true}  --> {"Microlith57MiscellaneousMechanics", "FrostHelper"}
  > mu.assoc {self = false} --> {}
]]

local associations = {
  {"Microlith57MiscellaneousMechanics", self = true},
  {"FrostHelper", expr = true},
}

function mu.assoc(tbl)
  if tbl.self == nil then tbl.self = true end
  local result = {}
  for _, a in ipairs(associations) do
    local found = false
    for k, _ in pairs(a) do
      if type(k) == "string" and tbl[k] then
        found = true
        break
      end
    end
    if found then
      table.insert(result, a[1])
    end
  end
  return result
end

---

--[[
  language file builder.

  it's annoying to maintain three separate files per entity (csharp, lua, lang).
  this is part of a system to build language entries inside plugins' lua code,
  resulting in only two places to coordinate, and allowing entries to be written
  in a more powerful and convenient way.
  it's useful to be able to just set keys without worrying about creating the
  intermediate tables, so this metatable magic allows that. (it's inspired by
  the nixpkgs module system's attrset merging!)

  given:
  > local l = mu.lang()
  > l.path.to.some.key = "value"
  > l.merge = { [1] = 1 }
  > l.merge = { [2] = 2 }
  > l.merge[3] = 3

  yields:
  > {
  >   path = { to = { some = { key = "value" } } },
  >   merge = { 1, 2, 3 }
  > }
]]

local function prepare_lang_entry(val)
  -- todo: rip off click's docstring reformatting
  return tostring(val)
    :gsub("^[ ]*", "")
    :gsub("[ \n]+$", "")
    :gsub("\n[ ]*", "\\n")
end

local lang_mt = {}
function lang_mt:__index(key)
  local val = {}
  setmetatable(val, lang_mt)
  rawset(self, key, val)
  return val
end
function lang_mt:__newindex(key, val)
  if type(val) == "table" then
    local current_val = self[key]
    for k, v in pairs(val) do
      current_val[k] = v
    end
  else
    rawset(self, key, prepare_lang_entry(val))
  end
end

function mu.lang(l)
  l = l or {}
  setmetatable(l, lang_mt)
  return l
end

function mu.print_lang(l)
  local function walk(node, prefix)
    local keys = table.keys(node)
    table.sort(keys)

    for _, k in ipairs(keys) do
      local v = node[k]
      local pfx = k
      if prefix then
        pfx = prefix .. "." .. pfx
      end

      if type(v) == "table" then
        walk(v, pfx)
      else
        print(pfx .. "=" .. v)
      end
    end
  end

  walk(l)
end

---

--[[
  the variants system.

  a lot of the entities in this mod are _parameterised_; that is, they have
  multiple CustomEntity names (with different associated fields) that all
  produce instances of the same, or conceptually related, classes.
  this often leads to situations where a lot of plugin code and lang entries
  describe very similar stuff, with only minor variations.
  the point of these functions is to collapse that redundancy into a single
  definition handling all cases.

  given:
  > mu.variants(
  >   "Example",
  >   {{"A", "B"}},
  >   {{"", "Expression"}, {"slider", "expression"}},
  > )

  yields:
  > {
  >   {name="Microlith57Misc/Example_A",            [0] = {"A"}, [1] = {nil, "slider"}},
  >   {name="Microlith57Misc/Example_A_Expression", [0] = {"A"}, [1] = {"Expression", "expression"}},
  >   {name="Microlith57Misc/Example_B",            [0] = {"B"}, [1] = {nil, "slider"}},
  >   {name="Microlith57Misc/Example_B_Expression", [0] = {"B"}, [1] = {"Expression", "expression"}},
  > }
  
  you can name properties:
  > local variants = mu.variants(
  >   "Example2",
  >   { {"A", "B"}, named = {"a", "b"} })
  > )

  and then retrieve these named properties in several ways:
  > local v = variants[1]
  > v[1].named --> "a"
  > v.named    --> "a"
  > v"something including {a}" --> "something including a"
]]

local var_mt = {}
function var_mt:__call(s)
  -- todo: "{{a}}" -> "{a}"
  -- todo: error messages
  return s:gsub("{[%a%d]+}", function(match) return tostring(self[match:sub(2, -2)]) end)
end

function mu.variants(name, ...)
  local vars = {...}

  local function build(variants, var)
    local new_variants = {}

    for i, _ in ipairs(var[1]) do
      local choices = {}
      for j, _ in pairs(var) do
        choices[j] = var[j][i]
      end

      for _, variant in ipairs(variants) do
        local new_variant = table.shallowcopy(variant)
        table.insert(new_variant, choices)
        table.insert(new_variants, new_variant)
      end
    end

    return new_variants
  end

  local variants = {{}} -- the empty product is 1

  for _, var in ipairs(vars) do
    variants = build(variants, var)
  end

  for _, variant in ipairs(variants) do
    local n = name
    for _, var in ipairs(variant) do
      local part = var[1]
      if part and part ~= "" then
        n = n .. "_" .. tostring(part)
      end

      for k, v in pairs(var) do
        if type(k) == "string" then
          variant[k] = v
        end
      end
    end
    variant.name = n
    setmetatable(variant, var_mt)
  end

  return variants
end

---

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
  > return {
  >   name = self.name,
  >   placements = {
  >     self(),
  >     self {
  >       "differentPlacement",
  >       name = "Example (Different)",
  >       data = { another = "different" }
  >     }
  >   },
  >   fieldOrder = self.fieldOrder,
  >   fieldInformation = self.fieldInformation
  > }
]]

local builder_mt = {}
local field_mt = {}

function builder_mt:__index(key)
  if builder_mt[key] then return builder_mt[key] end
  local field = {
    _builder = self,
    _field = key
  }
  setmetatable(field, field_mt)
  return field
end
function builder_mt:__newindex(key, val)
  if builder_mt[key] then
    builder_mt[key](val)
    return self
  end
  if not self._order_set[key] then
    self._order_set[key] = true
    table.insert(self.fieldOrder, key)
  end
  self._fields[key] = val
end
function builder_mt:_lang(key)
  if mu.preprocess then
    rawset(self, "_lang", mu.preprocess.lang[key][self.name])
  end
  return self
end
function builder_mt:_xy()
  self.x = nil self.y = nil
  return self
end
function builder_mt:_rect()
  self.x = nil self.y = nil self.width = nil self.height = nil
  return self
end
function builder_mt:_depth(depth)
  if depth == false then return self end
  self.depth = depth
  self.depth:optional():int()
  return self
end
function builder_mt:_tags(tags)
  if tags == false then return self end
  tags = tags or {"PauseUpdate", "FrozenUpdate", "TransitionUpdate"}
  self.tags = ""
  self.tags.info = {
    fieldType = "list",
    elementSeparator = ",",
    elementOptions = {
      options = tags,
      warningValidator = function(v)
        v = v:gsub("^%s*", ""):gsub("%s+$", "")
        for _, t in ipairs(tags) do
          if v == t then return true end
        end
      end
    },
    valueTransformer = function(vs)
      local tags = {}
      local tags_set = {}
      local vt = string.split(vs, ",")()
      for _, v in pairs(vt) do
        v = v:gsub("^%s*", ""):gsub("%s+$", "")
        if not tags_set[v] then
          table.insert(tags, v)
          tags_set[v] = true
        end
      end
      return table.concat(tags, ",")
    end
  }
  self.tags.desc = "Additional tags for this entity."
end

field_mt.__index = field_mt
function field_mt:__newindex(k, v)
  self[k](self, v)
end
function field_mt:name(name)
  if mu.preprocess then
    self._builder._lang.attributes.name[self._field] = name
  end
  return self
end
function field_mt:desc(desc)
  if mu.preprocess then
    self._builder._lang.attributes.description[self._field] = desc
  end
  return self
end
function field_mt:info(info)
  local i = self._builder.fieldInformation[self._field] or {}
  for k, v in pairs(info) do
    i[k] = v
  end
  self._builder.fieldInformation[self._field] = i
  return self
end
function field_mt:validator(val)
  return self:info{validator = val}
end
function field_mt:nonempty()
  return self:validator(mu.validate_nonempty)
end
function field_mt:optional()
  self:info {allowEmpty = true}
end
function field_mt:int()
  return self:info {fieldType = "integer"}
end

local function default_placement_name(name)
  return name
    :gsub("^" .. mu.modname .. "/", "")
    :gsub("_.*", "")
    :gsub("^.", string.lower)
end

local function group_name(name)
  return name
    :gsub("_.*", "")
end

function builder_mt:__call(tbl)
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
  return {
    name = name,
    data = data,
    ext_group = group_name(self.name)
  }
end

local function prepare_name(name)
  if not name:match("^" .. mu.modname .. "/")  then
    name = mu.modname .. "/" .. name
  end
  return name
end

function mu.builder(tbl)
  local name = prepare_name(tbl[1])
  local builder = {
    name = name,
    _name = tbl.name,
    _desc = tbl.desc,
    _fields = {},
    _order_set = {},
    fieldOrder = {},
    fieldInformation = {}
  }
  setmetatable(builder, builder_mt)
  return builder
end
function mu.entity(tbl)
  return mu.builder(tbl)
    :_lang("entities")
    :_xy()
    :_depth(tbl.depth)
    :_tags(tbl.tags)
end
function mu.trigger(name)
  return mu.builder(tbl)
    :_lang("triggers")
    :_rect()
end

---

return mu
