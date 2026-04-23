local variants = mu.variants(
  "SliderCameraZoomController",
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
    name = v"Camera Zoom Controller ({Noun})",
    desc = "Sets the camera zoom based on a slider value."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the zoom"} 

  self.focusX "160.0"
    :nonempty()
    :desc "Slider containing the focus point's screen-space X position."

  self.focusY "90.0"
    :nonempty()
    :desc "Slider containing the focus point's screen-space Y position."

  self.amount "1.0"
    :nonempty()
    :desc(v"{Noun} containing the amount to zoom by, in [1.0, ∞).")

  result[i] = self()
end
return result
