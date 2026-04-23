local variants = mu.variants(
  "SliderCameraPaddingController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Camera Padding Controller ({Noun})",
    desc = "Sets the camera padding (eg. used in the player seeker cutscene) based on a slider value."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the padding"}

  self.amount "32.0"
    :nonempty()
    :desc(v[[
      {Noun} containing the amount to pad the screen by, in [0.0, ∞). The amount in the vanilla player seeker cutscene is 32.0.
    ]])

  result[i] = self()
end
return result
