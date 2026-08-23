local variants = mu.variants(
  "SliderTrigger",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local name = v"Slider Trigger{ (Expr?)}"
  local self = mu.trigger {
    v.name,
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
