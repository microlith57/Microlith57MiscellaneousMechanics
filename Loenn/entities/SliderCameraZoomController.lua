local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  focusX = {validator = nonEmptyValidator},
  focusY = {validator = nonEmptyValidator},
  amount = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "focusX", "focusY",
  "amount"
}

return {
  {
    name = "Microlith57Misc/SliderCameraZoomController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_zoom_controller",
    placements = {
      {
        name = "sliderCameraZoomController",
        data = {
          flag = "",
          invertFlag = false,
          focusX = "160.0",
          focusY = "90.0",
          amount = "1.0"
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/SliderCameraZoomController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_zoom_controller",
    placements = {
      {
        name = "sliderCameraZoomController",
        data = {
          expression = "",
          focusX = "160.0",
          focusY = "90.0",
          amount = "1.0"
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
