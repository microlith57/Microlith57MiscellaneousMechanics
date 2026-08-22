local variants = mu.variants(
  "SliderCameraOffsetTrigger",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local name = v"Camera Target{ (Expr?)}"
  local self = mu.trigger {
    "SliderTrigger",
    name = name,
  }
  self:_flag_or_expr {v.noun, action = "set the slider"}

  self.slider "slider"
    :nonempty()
    :desc("The slider to set.")

  self:_position_mode {action = "set the slider"}

  self.from(0)
    :desc("The value to set the slider to at the 'from' end of the direction.")
  self.to(1)
    :desc("The value to set the slider to at the 'to' end of the direction.")

  result[i] = self {
    triggerText = name
  }
end
return result
