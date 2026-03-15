if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "SliderAudioParamController",
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
    name = v"Audio Param Controller ({Noun})",
    desc = "Sets an audio (music or ambience) param based on a slider value."
  }

  self[v.noun] = ""
  self[v.noun].desc = v"If present, set the param only when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.param = ""
  self.param.desc = "Name of the param to set."

  self.isAmbience = false
  self.isAmbience.desc = "If checked, set an ambience param; otherwise, set a music param."

  self.value = "0.0"
  self.value:nonempty()
  self.value.desc = v"{Noun} to set the param to."

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.Noun == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_audio_param_controller",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
end
return result
