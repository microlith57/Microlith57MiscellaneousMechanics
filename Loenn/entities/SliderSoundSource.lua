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
  params = listInformation
}

local fieldOrder = {
  "x", "y",
  "enableFlag", "invertEnable", "enableExpression",
  "playingFlag", "invertPlaying", "playingExpression",
  "positionX", "positionY", "positionRelative",
  "params", "volume",
  "sound",
  "globalRoomCompat"
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
          positionX = "",
          positionY = "",
          positionRelative = true,
          params = "",
          volume = "1.0",
          sound = "",
          globalRoomCompat = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
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
          positionX = "",
          positionY = "",
          positionRelative = true,
          params = "",
          volume = "1.0",
          sound = "",
          globalRoomCompat = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
