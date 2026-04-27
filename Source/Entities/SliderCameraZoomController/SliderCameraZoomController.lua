local variants = mu.variants(
  "SliderCameraZoomController",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Camera Zoom Controller ({Float})",
    desc = v"Sets the camera zoom based on the value of {a float}."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the zoom"}

  self.focusX "160.0"
    :nonempty()
    :desc(v"{Float} {containing} the focus point's screen-space X position.")

  self.focusY "90.0"
    :nonempty()
    :desc(v"{Float} {containing} the focus point's screen-space Y position.")

  self.amount "1.0"
    :nonempty()
    :desc(v"{Float} {containing} the amount to zoom by, in [1.0, ∞).")

  result[i] = self()
end
return result
