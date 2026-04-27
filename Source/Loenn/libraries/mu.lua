local mu = (mu and mu.preprocess and mu) or {}

--local serialize = require("utils.serialize").serialize

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
mu.modpathsegment = "microlith57/misc"

function mu.validate_nonempty(s) return s ~= "" end

function mu.pp(o)
  local success, val = serialize(o)
  print(val)
end

---

--[[
  preprocessing stuff.

  most of this only makes sense when run through the preprocess entrypoint, but
  sometimes there is still a return value when run from loenn.

  note that there are other preprocessing-related features in other sections.
]]

function mu.defer(f)
  if mu.preprocess then table.insert(mu.preprocess.feature.defer, f) end
end

local function abs_path_for(rel)
  return mu.preprocess.self.feat_path .. "/" .. rel:gsub("^/", '')
end

function mu.plan_move(tbl)
  if not mu.preprocess then return end

  local rel = tbl[1]
  local src = abs_path_for(rel)

  if not mu.preprocess.planned_moves[src] then mu.preprocess.planned_moves[src] = {} end
  local move = mu.preprocess.planned_moves[src]

  local dst = tbl[2]
  if dst == false then
    move.to = nil
  elseif dst ~= nil then
    move.to = dst
  end

  if tbl.header == true then
    move.header = nil
  elseif tbl.header == false then
    move.header = false
  end

  local feature = mu.preprocess.self.feature
  if move.feature and move.feature ~= feature then
    error("feature " .. feature .. " tried to overwrite file " .. dst .. " which is already set by " .. move.feature)
  end
  move.feature = feature

  return dst
end
function mu.plan_move_self(a)
  if not mu.preprocess then return end

  local tbl = type(a) == "table" and a or {a}
  tbl[2] = tbl[1]
  tbl[1] = mu.preprocess.self.rel_path
  return mu.plan_move(tbl)
end

function mu.plan_only_editor(dst)
  if not mu.preprocess then return end

  mu.preprocess.everestignore[dst] = true
end

function mu.texture(tbl)
  local src = tbl[1]
  local dst = tbl[2]
  if not dst then dst = "objects/" .. mu.modpathsegment .. "/" .. src end

  local atlas = tbl.atlas or "Gameplay"
  local abs_dst = "Graphics/Atlases/" .. atlas .. "/" .. dst .. ".png"

  mu.plan_move {src .. ".png", abs_dst}
  if tbl.only_editor then mu.plan_only_editor(abs_dst) end

  return dst
end

---

--[[
  string formatter.

  usage:
  > local f = mu.fmt { example = "a", another = "b" }
  > f"{example}, {another}" --> "a, b"

  a more complicated example, using mu.vary:
  > local channels = mu.vary {
  >   col = {"r", "g", "b", "a"},
  >   name = {"red", "green", "blue", "premultiplied alpha"},
  > }
  >
  > local res = {}
  > local f = mu.fmt {Noun = "Expression"}
  > for _, c in ipairs(channels) do
  >   c(f) -- merge the contents of f into c
  >   res[c.col] = c"{Noun} for the {name} component."
  > end

  yields:
  > {
  >   r = "Expression for the red component."
  >   g = "Expression for the green component."
  >   b = "Expression for the blue component."
  >   a = "Expression for the premultiplied alpha component."
  > }
]]

local Fmt = {}
function Fmt:__call(o)
  if type(o) == "string" then
    return self:_format(o)
  elseif type(o) == "table" then
    return self:_merge(o)
  end
  return o
end
function Fmt:__index(key)
  local meta = rawget(Fmt, key)
  if meta then return meta end
  local val = rawget(self, key)
  if val then return val end

  if type(key) ~= "string" then return end

  local upper = false
  key = key:gsub("%a", function(a)
    if a:match("%u") then upper = true end
    return a:lower()
  end, 1)

  local a = false
  key = key:gsub("^an? ", function()
    a = true;
    return ""
  end)

  local val = rawget(self, key)
  if not val then return end
  val = tostring(val)

  if a then
    local vowel = val:match("^[aeiou]")
    val = (vowel and "an " or "a ") .. val
  end

  if upper then
    val = val:gsub("%l", string.upper, 1)
  end

  return val
end
local punct_precedence = {[","] = 1, [";"] = 2, ["-"] = 3}
function Fmt:_format_one(s)
  local open = s:match("^%s*[%(%)%[%]]*") or ""
  local close = s:match("[%(%)%[%]]*%s*$") or ""
  local c = s:sub(#open + 1, -1 - #close)
  
  local res = {}
  local i = 1
  local punct = ""
  while true do
    local next = c:find("[,;-]", i)
    local j = next and next - 1 or #c
    local key = c:sub(i, j):gsub("^%s+", ""):gsub("%s+$", "")
    local val = self[key]
    if val == nil then error(("key %s not found in format string"):format(key)) end

    local found = val ~= ""
    if found then
      local p = punct
      if punct ~= "" then p = p .. " " end
      if punct == "-" then p = " " .. p end
      if #res > 0 then table.insert(res, p) end
      table.insert(res, val)
    end

    i = j + 2
    if i > #c then break end

    local next_punct = next and c:sub(next, next) or ""
    local prev_prec = punct_precedence[punct] or -2
    local next_prec = punct_precedence[next_punct] or -1
    if found or next_prec > prev_prec then
      punct = next_punct
    end
  end
  if #res > 0 then
    return open .. table.concat(res) .. close
  else
    return ""
  end
end
function Fmt:_format(s)
  local res = {}
  local i = 1
  while true do
    local open = s:find("{", i)
    if not open then
      local t = s:sub(i):gsub("}}", "}")
      table.insert(res, t)
      break
    elseif s[open + 1] == "{" then
      local t = s:sub(i, open - 1)
      table.insert(res, "{")
      i = open + 2
    else
      if open > i then
        local t = s:sub(i, open - 1):gsub("}}", "}")
        table.insert(res, t)
      end
      local close = s:find("}", open + 1)
      if not close then error(("unbalanced curly braces in format string, starting at %d"):format(open)) end
      if open == close then error(("empty format specifier at %d"):format(open)) end
      local contents = s:sub(open + 1, close - 1)
      table.insert(res, self:_format_one(contents))
      i = close + 1
    end
  end
  return table.concat(res)
end
function Fmt:_merge(t)
  for k, v in pairs(t) do
    self[k] = v
  end
  return self
end

function mu.fmt(t)
  setmetatable(t, Fmt)
  return t
end

function mu.vary(tbl)
  local result = {}
  local _, ref = next(tbl)
  for i, _ in ipairs(ref) do
    local r = mu.fmt {}
    for k, v in pairs(tbl) do
      r[k] = v[i]
    end
    table.insert(result, r)
  end
  return result
end

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

-- todo: deal with case where same entry is set twice

local indent_ruler = 80

local function prepare_lang_entry(val)
  local lines = tostring(val):split("\n")()
  if #lines == 0 then return "" end
  local indent = #(lines[1]:match("^%s*"))

  local res = {}
  local paragraph = {}
  local reindent = true

  local function push_paragraph()
    if #paragraph == 0 then return end

    if #res > 0 then table.insert(res, "") end

    if reindent then
      local words = {}
      for _, line in ipairs(paragraph) do
        local words_in_line = line:split(" ")()
        for _, word in ipairs(words_in_line) do
          table.insert(words, word)
        end
      end

      local line = {}
      local len = 0
      local function push_line()
        table.insert(res, table.concat(line, " "))
        line = {}
        len = 0
      end

      for _, word in ipairs(words) do
        local next_len = len + #word + 1
        if next_len <= indent_ruler then
          table.insert(line, word)
          len = next_len
        else
          push_line()

          table.insert(line, word)
          len = #word

          if len > indent_ruler then push_line() end
        end
      end
      push_line()
    else
      for _, line in ipairs(paragraph) do
        table.insert(res, line)
      end
    end
    paragraph = {}
    reindent = true
  end

  for _, line in ipairs(lines) do
    local this_indent = #(line:match("^%s*")) - indent
    if this_indent < 0 then this_indent = 0 end

    line = line:gsub("^%s*", ""):gsub("%s*$", "")
    if line == "\b" or line == "\\b" then
      reindent = false
    elseif line == "" then
      push_paragraph()
    else
      table.insert(paragraph, line)
    end
  end
  push_paragraph()

  return table.concat(res, "\\n")
end

local Lang = {}
function Lang:__index(key)
  local val = {}
  setmetatable(val, Lang)
  rawset(self, key, val)
  return val
end
function Lang:__newindex(key, val)
  if type(val) == "table" then
    local current_val = self[key]
    for k, v in pairs(val) do
      current_val[k] = v
    end
  elseif val == nil then
    rawset(self, key, nil)
  else
    rawset(self, key, prepare_lang_entry(val))
  end
end

function mu.lang(l)
  l = l or {}
  setmetatable(l, Lang)
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
      elseif type(v) == "string" then
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
  >   {{"A", "B"}, named = {"a", "b"}})
  > )

  and then retrieve these named properties in several ways:
  > local v = variants[1]
  > v[1].named --> "a"
  > v.named    --> "a"
  > v"something including {named}" --> "something including a"
]]

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
    mu.fmt(variant)
  end

  return variants
end

function mu.var_expr(tbl)
  tbl = tbl or {}
  local res = {
    {tbl[1] or "", tbl[2] or "Expression"},
    bool = {"flag", "expression"},
    set = {"set", "truthy"},
    unset = {"unset", "falsy"},
    int = {"counter", "expression"},
    float = {"slider", "expression"},
    containing = {"containing", "yielding"},
    ["expr?"] = {"", "expression"},
    ["exp?"] = {"", "expr"},
  }

  for k, v in pairs(tbl) do
    if type(k) == "string" then
      res[k] = v
    end
  end

  return res
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
  > return self {
  >   {}, -- default placement (inherits name, desc, data)
  >   {
  >     "differentPlacement",
  >     name = "Example (Different)",
  >     data = { another = "different" }
  >   },
  > }
]]

local Builder = {}
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
        return false
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
  tbl.format = tbl.format or "If present, only {imperative} when this {bool} is {set}."
  mu.fmt(tbl)

  local name = tbl.name and tbl(tbl.name) or tbl.bool
  local invert = tbl.invert and tbl(tbl.invert) or "invertFlag"
  local defaultInvert = false
  if tbl.defaultInvert ~= nil then defaultInvert = tbl.defaultInvert end

  self[name] = tbl.default or ""
  self[name].desc = tbl.desc or tbl(tbl.format)
  if not expr and (tbl.invertFlag ~= false) then self[invert] = defaultInvert end

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

Field.__index = Field
function Field:__call(default)
  return self:default(default)
end
function Field:__newindex(k, v)
  local f = Field[k]
  if not f then error(("attempt to set %s on field %s"):format(k, self._field)) end
  f(self, v)
end
function Field:default(default)
  self._builder[self._field] = default
  return self
end
function Field:name(name)
  if mu.preprocess then
    self._builder._lang.attributes.name[self._field] = name
  end
  return self
end
function Field:desc(desc)
  if mu.preprocess then
    self._builder._lang.attributes.description[self._field] = desc
  end
  return self
end
function Field:info(info)
  local i = self._builder._info[self._field] or {}
  for k, v in pairs(info) do
    i[k] = v
  end
  self._builder._info[self._field] = i
  return self
end
function Field:validator(val)
  return self:info{validator = val}
end
function Field:nonempty()
  return self:validator(mu.validate_nonempty)
end
function Field:optional()
  return self:info {allowEmpty = true}
end
function Field:int()
  return self:info {fieldType = "integer"}
end
function Field:list(tbl)
  local options = {}
  for i, o in ipairs(tbl) do options[i] = o end

  local editable = tbl.editable or false

  return self:info {
    options = options,
    editable = editable,
  }
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
      local doc = type(self._lang.attributes.description[f]) == "string"
      if not doc and not allowed_undoc[f] and not (f:match("^invert")) then
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

local function prepare_name(name)
  if not name:match("^" .. mu.modname .. "/")  then
    name = mu.modname .. "/" .. name
  end
  return name
end

local function unprepare_name(name)
  return name:gsub("^" .. mu.modname .. "/", ""):gsub("_.*", "")
end

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
function mu.entity(tbl)
  return mu.builder(tbl)
    :_lang("entities")
    :_xy()
    :_depth(tbl.depth)
    :_tags(tbl.tags)
end
function mu.controller(tbl)
  local self = mu.entity(tbl)
  self:_extra {depth = -1000000}

  tbl.texture = (tbl.texture ~= nil) and tbl.texture or {only_editor = true}
  if type(tbl.texture) == "string" then tbl.texture = {tbl.texture, only_editor = true} end
  if tbl.texture then
    self:_texture(tbl.texture)
  end

  return self
end
function mu.trigger(tbl)
  return mu.builder(tbl)
    :_lang("triggers")
    :_rect()
end

---

return mu
