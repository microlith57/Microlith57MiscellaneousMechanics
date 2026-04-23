local variants = mu.variants(
  "FreezeTimeActiveController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
    par = {"", "(Expression)"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Freeze TimeActive Controller {par}",
    desc = "Freezes the Scene.TimeActive field; has some wacky effects."
  }
  self:_flag_or_expr {v.noun, imperative = "freeze", default = "freezeTimeActive"}

  result[i] = self()
end
return result
