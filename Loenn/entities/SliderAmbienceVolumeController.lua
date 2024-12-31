local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  slider = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "volume"
}

return {
  {
    name = "Microlith57Misc/SliderAmbienceVolumeController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_ambience_volume_controller",
    placements = {
      {
        name = "sliderAmbienceVolumeController",
        data = {
          flag = "",
          invertFlag = false,
          volume = "1.0"
        }
      }
    },
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/SliderAmbienceVolumeController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_ambience_volume_controller",
    placements = {
      {
        name = "sliderAmbienceVolumeController",
        data = {
          expression = "",
          volume = "1.0"
        }
      }
    },
    fieldOrder = fieldOrder
  }
}
