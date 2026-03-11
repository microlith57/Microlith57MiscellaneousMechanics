local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  amount = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "amount"
}

return {
  {
    name = "Microlith57Misc/SliderCameraPaddingController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_padding_controller",
    placements = {
      {
        name = "sliderCameraPaddingController",
        data = {
          flag = "",
          invertFlag = false,
          amount = "32.0"
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/SliderCameraPaddingController_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_padding_controller",
    placements = {
      {
        name = "sliderCameraPaddingController",
        data = {
          expression = "",
          amount = "32.0"
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
