if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "FreezeTimeActiveController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
    par = {"", "(Expression)"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Freeze TimeActive Controller {par}",
    desc = "Freezes the Scene.TimeActive field; has some wacky effects."
  }

  self[v.noun] = "freezeTimeActive"
  self[v.noun].desc = v"If present, freeze only when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.Noun == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/freeze_time_active_controller",
    placements = {self()},
    fieldOrder = self.fieldOrder,
    fieldInformation = self.fieldInformation
  }
end
return result
