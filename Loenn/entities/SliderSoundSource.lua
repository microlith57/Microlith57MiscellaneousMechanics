local utils = require("utils")

-- todo: art

local listInformation = {
  fieldType = "list",
  elementSeparator = ",",
  elementDefault = "param:value",
  elementOptions = {
    fieldType = "list",
    elementSeparator = ":",
    minimumElements = 2,
    maximumElements = 2
  }
}

local fieldInformation = {
  paramSliders = listInformation,
  paramExpressions = listInformation
}

return {
  {
    name = "Microlith57Misc/SliderSoundSource",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_sound_source",
    placements = {
      {
        name = "sliderSoundSource",
        data = {
          enableFlag = "",
          invertEnable = false,
          playingFlag = "",
          invertPlaying = false,
          positionSliderX = "",
          positionSliderY = "",
          positionRelative = true,
          paramSliders = "",
          volumeSlider = "1",
          sound = ""
        }
      }
    },
    fieldInformation = fieldInformation
  },
  {
    name = "Microlith57Misc/SliderSoundSource_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_sound_source",
    placements = {
      {
        name = "sliderSoundSource",
        data = {
          enableExpression = "",
          playingExpression = "",
          positionExpressionX = "",
          positionExpressionY = "",
          positionRelative = true,
          paramExpressions = "",
          volumeExpression = "",
          sound = ""
        }
      }
    },
    fieldInformation = fieldInformation
  }
}
