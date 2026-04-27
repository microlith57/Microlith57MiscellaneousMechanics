local variants = mu.variants(
  "SliderCameraPaddingController",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Camera Padding Controller ({Float})",
    desc = v"Sets the camera padding (eg. used in the player seeker cutscene) based on the value of {a float}."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the padding"}

  self.amount "32.0"
    :nonempty()
    :desc(v[[
      {Float} containing the amount to pad the screen by, in [0.0, ∞).
      The amount in the vanilla player seeker cutscene is 32.0.
    ]])

  result[i] = self()
end
return result
