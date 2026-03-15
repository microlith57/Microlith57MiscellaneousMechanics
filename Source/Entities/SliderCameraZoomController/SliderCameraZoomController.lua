if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

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
  local self = mu.entity {
    v.name,
    name = v"Camera Zoom Controller ({Noun})",
    desc = "Sets the camera zoom based on a slider value."
  }

  self[v.noun] = ""
  self[v.noun].desc = v"If present, only affect the zoom when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.focusX = "160.0"
  self.focusX:nonempty()
  self.focusX.desc = "Slider containing the focus point's screen-space X position."

  self.focusY = "90.0"
  self.focusX:nonempty()
  self.focusY.desc = "Slider containing the focus point's screen-space Y position."

  self.amount = "1.0"
  self.amount:nonempty()
  self.amount.desc = v"{Noun} containing the amount to zoom by, in [1.0, ∞)."

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.Noun == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_zoom_controller",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
end
return result
