local self = mu.controller {
  "SliderCameraPositionGetter",
  name = "Slider Camera Position Getter",
  desc = "Sets sliders based on the camera's position."
}
self:_flag_or_expr {"flag", imperative = "set update the slider values"}

self.sliderPrefix "cameraPosition"
  :desc "X and Y will be appended to this value to get the slider names for the position."

self.tracking "Position"
  :list {
    "Position",
    "Origin",
    "BottomLeft",
    "BottomRight",
    "TopLeft",
    "TopRight",
    "Center",
  }
  :desc "What position to track."

return self()
