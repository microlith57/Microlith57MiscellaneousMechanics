local mu = (mu and mu.preprocess and mu) or {}

---

mu.modname = "Microlith57Misc"

function mu.validate_nonempty(s) return s ~= "" end

---

--[[
  language file builder.

  it's annoying to maintain three separate files per entity (csharp, lua, lang).
  this is part of a system to build language entries inside plugins' lua code,
  resulting in only two places to coordinate, and allowing entries to be written
  in a more powerful and convenient way.
  it's useful to be able to just set keys without worrying about creating the
  intermediate tables, so this metatable magic allows that. (it's inspired by
  the nixpkgs module system!)

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

local lang_mt = {}

function lang_mt.__index(self, key)
  local val = {}
  setmetatable(val, lang_mt)
  rawset(self, key, val)
  return val
end

function lang_mt.__newindex(self, key, val)
  if type(val) == "table" then
    local current_val = self[key]
    for k, v in pairs(val) do
      current_val[k] = v
    end
  else
    rawset(self, key, val)
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
        print(pfx .. "=" .. tostring(v:gsub("^[ ]*", ""):gsub("[ ]+$", ""):gsub("\n%s*", "\\n")))
      end
    end
  end

  walk(l)
end

---

--[[
  entity/trigger/etc table builder.
]]

local builder_mt = {}
builder_mt.__index = builder_mt

function builder_mt.__newindex(self, field, value)
  self:push(field)
  self._fields[field] = value
end

function builder_mt.push(self, field)
  if not self._order_set[field] then
    self._order_set[field] = true
    table.insert(self.fieldOrder, field)
  end
  return self
end

function builder_mt.xy(self) self:push("x"):push("y") end
function builder_mt.rect(self, width, height)
  self:xy()
  self.width = width or 16
  self.height = height or 16
  return self
end

function builder_mt.info(self, field, info)
  if not self.fieldInformation[field] then self.fieldInformation[field] = {} end
  for k, v in pairs(info) do
    self.fieldInformation[field][k] = v
  end
  return self
end

function builder_mt.validate(self, field, validator) return self:info(field, {validator = validator}) end
function builder_mt.nonempty(self, field) return self:validate(field, mu.validate_nonempty) end

function builder_mt.placement(self, name, override)
  local data = table.shallowcopy(self._fields)
  for k, v in pairs(override or {}) do
    data[k] = v
  end
  return {
    name = name,
    data = data
  }
end

function mu.builder()
  local builder = {
    _fields = {},
    _order_set = {},
    fieldOrder = {},
    fieldInformation = {}
  }
  setmetatable(builder, builder_mt)
  return builder
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
  >  mu.variants(
  >    "Example",
  >    {{"A", "B"}},
  >    {{"", "Expression"}, {"slider", "expression"}},
  >  )

  yields:
  > {
  >   {name="Microlith57Misc/Example_A",            [0] = {"A"}, [1] = {nil, "slider"}},
  >   {name="Microlith57Misc/Example_A_Expression", [0] = {"A"}, [1] = {"Expression", "expression"}},
  >   {name="Microlith57Misc/Example_B",            [0] = {"B"}, [1] = {nil, "slider"}},
  >   {name="Microlith57Misc/Example_B_Expression", [0] = {"B"}, [1] = {"Expression", "expression"}},
  > }
]]

function mu.variants(name, ...)
  local vars = {...}

  local function build(variants, var)
    local new_variants = {}

    for i, _ in ipairs(var[1]) do
      local choices = {}
      for j in ipairs(var) do
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
    local n = mu.modname .. "/" .. name
    for _, var in ipairs(variant) do
      local part = var[1]
      if part and part ~= "" then
        n = n .. "_" .. tostring(part)
      end
    end
    variant.name = n
  end

  return variants
end

---

return mu
