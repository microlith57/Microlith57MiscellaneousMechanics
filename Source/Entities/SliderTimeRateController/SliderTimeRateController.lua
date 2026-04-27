local variants = mu.variants(
  "SliderTimeRateController",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Time Rate Controller ({Float})",
    desc = v"Modifies how fast time progresses based on the value of {a float}."
  }
  self:_flag_or_expr {v.bool, imperative = "affect the time rate"}

  self.multiplier "1.0"
    :nonempty()
    :desc(v"{Float} {containing} the amount to multiply the time rate by, in [0.0, ∞).")

  result[i] = self()
end
return result

