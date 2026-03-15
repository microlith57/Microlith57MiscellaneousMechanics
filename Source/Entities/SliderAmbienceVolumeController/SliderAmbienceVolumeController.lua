if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "SliderAmbienceVolumeController",
  {
    {"", "Expression"},
    typ  = {"Slider", "Expression"},
    noun = {"flag", "expression"},
    adj  = {"set", "truthy"}
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Ambience Volume Controller ({typ})"
  }

  self.volume = "1.0"
  self.volume:nonempty()
  self.volume.desc = v"{typ} to set the volume to."

  self[v.noun] = ""
  self[v.noun].desc = v"If present, only set the volume if the {noun} is {adj}."

  if v.typ == "Slider" then
    self.invertFlag = false
  end

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.Noun == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_ambience_volume_controller",
    placements = {self()},
    fieldOrder = self.fieldOrder,
    fieldInformation = self.fieldInformation,
  }
end
return result
