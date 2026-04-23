local variants = mu.variants(
  "SliderTimeRateController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Time Rate Controller ({Noun})",
    desc = "Modifies how fast time progresses based on a slider value."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the time rate"}

  self.multiplier "1.0"
    :nonempty()
    :desc(v"{Noun} containing the amount to multiply the time rate by, in [0.0, ∞).")

  result[i] = self()
end
return result
