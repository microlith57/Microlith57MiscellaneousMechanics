if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local self = mu.entity {
  v.name,
  name = "Slider Camera Position Getter",
  desc = "Sets sliders based on the camera's position."
}

self.flag = ""
self.flag.desc = "If present, only update the slider values if the flag is set."
self.invertFlag = false

self.sliderPrefix = "cameraPosition"
self.sliderPrefix.desc = "X and Y will be appended to this value to get the slider names for the position."

self.tracking = "Position"
self.tracking.info = {
  options = {
    "Position",
    "Origin",
    "BottomLeft",
    "BottomRight",
    "TopLeft",
    "TopRight",
    "Center",
  },
  editable = false
}
self.tracking.desc = "What position to track."

return {
  {
    name = self.name,
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_position_getter",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
}
