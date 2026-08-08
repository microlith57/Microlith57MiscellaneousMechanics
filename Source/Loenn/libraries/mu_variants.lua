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

---@class Var
---@field [1] string[]
---@field [integer|string] any[]?

---@class Variant
---@field name string
---@field [string] any
---@field [integer] {[integer|string]: any}

---@param name string
---@param ... Var
---@return Variant[]
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

---@param tbl {[1]: string?, [2]: string?, [string]: any}?
---@return Var
function mu.var_expr(tbl)
  tbl = tbl or {}
  local res = {
    {tbl[1] or "", tbl[2] or "Expression"},
    bool = {"flag", "expression"},
    bools = {"flags", "expressions"},
    set = {"set", "truthy"},
    unset = {"unset", "falsy"},
    int = {"counter", "expression"},
    ints = {"counters", "expressions"},
    float = {"slider", "expression"},
    floats = {"sliders", "expressions"},
    containing = {"containing", "yielding"},
    ["expr?"] = {"", "expression"},
    ["exp?"] = {"", "expr"},
    ["e?"] = {"", "e"},
  }

  for k, v in pairs(tbl) do
    if type(k) == "string" then
      res[k] = v
    end
  end

  return res
end

---

---@class Typology
---@field _build fun(self: Typology): fun(table): string
---@field [string] TypologyTyp
local Typology = {}

---@class TypologyTyp
---@field private _typology Typology
---@field private _key any
---@operator call(table<any, string>): Typology
local TypologyTyp = {}

function Typology:__index(k)
  local meta = Typology[k]
  if meta then return meta end
  return setmetatable({_key = k}, TypologyTyp)
end
function Typology:_build()
  ---@param tbl table<string, any>
  return function(tbl)
    local res = {}
    for _, data in ipairs(self) do
      local key = tbl[data._key]
      local val = data[key]
      if val == nil then val = "?" end
      table.insert(res, val)
    end
    return table.concat(res)
  end
end

---@param tbl table<any, string>
function TypologyTyp.__call(self, typology, tbl)
  local data = table.shallowcopy(tbl)
  data._key = self._key
  table.insert(typology, data)
  return typology
end

---@return Typology
function mu.typology()
  return setmetatable({}, Typology)
end
