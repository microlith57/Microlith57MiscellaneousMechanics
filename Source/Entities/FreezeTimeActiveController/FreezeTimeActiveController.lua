local variants = mu.variants(
  "FreezeTimeActiveController",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Freeze TimeActive Controller {(Expr?)}",
    desc = "Freezes the Scene.TimeActive field; has some wacky effects."
  }
  self:_flag_or_expr {v.bool, imperative = "freeze", default = "freezeTimeActive"}

  result[i] = self()
end
return result
