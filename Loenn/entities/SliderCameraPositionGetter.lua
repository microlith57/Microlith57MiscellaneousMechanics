local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  sliderPrefix = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag",
  "sliderPrefix",
}

return {
  {
    name = "Microlith57Misc/SliderCameraPositionGetter",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_position_getter",
    placements = {
      {
        name = "sliderCameraPositionGetter",
        data = {
          flag = "",
          invertFlag = false,
          sliderPrefix = "cameraPosition"
        }
      }
    },
    fieldOrder = fieldOrder
  }
}
