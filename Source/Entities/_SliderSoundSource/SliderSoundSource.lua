if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "SliderColorgradeController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
    nadj = {"unset", "falsy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Slider Sound Source ({Noun})",
    desc = "Plays a sound, with position / volume / parameters controlled by sliders."
  }

  self["enable" .. v.Noun] = ""
  self["enable" .. v.Noun].desc = v"If present, stop the sound when the {noun} is {nadj}."
  if v.noun == "flag" then self.invertEnable = false end

  self["playing" .. v.Noun] = ""
  self["playing" .. v.Noun].desc = v"If present, pause the sound when the {noun} is {nadj}."
  if v.noun == "flag" then self.invertPlaying = false end

  self.positionX = ""
  self.positionX.desc = ""
  self.positionY = ""
  self.positionY.desc = ""

  self.positionRelative = true
  self.listener = "VanillaCamera"
  self.listener.desc = [[
    idk i don't know 
  ]]
end
return result



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
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
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
