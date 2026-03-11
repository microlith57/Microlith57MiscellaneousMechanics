local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  value = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "param", "isAmbience",
  "value"
}

return {
  {
    name = "Microlith57Misc/SliderAudioParamController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_audio_param_controller",
    placements = {
      {
        name = "sliderAudioParamController",
        data = {
          flag = "",
          invertFlag = false,
          param = "fade",
          isAmbience = false,
          value = "0.0"
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/SliderAudioParamController_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_audio_param_controller",
    placements = {
      {
        name = "sliderAudioParamController",
        data = {
          expression = "",
          param = "fade",
          isAmbience = false,
          value = "0.0"
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
