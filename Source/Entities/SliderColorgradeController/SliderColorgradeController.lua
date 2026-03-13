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
  }
)


local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Colourgrade Controller ({Noun})",
    desc = "Interpolates between colourgrades based on a slider value."
  }

  self[v.noun] = ""
  self[v.noun].desc = v"If present, only set the colourgrade when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.colorgradeA = "none"
  self.colorgradeA.name = "Colourgrade A"
  self.colorgradeA.desc = "Colourgrade to interpolate from (lerp = 0.0)."
  self.colorgradeB = "none"
  self.colorgradeA.name = "Colourgrade B"
  self.colorgradeB.desc = "Colourgrade to interpolate to (lerp = 1.0)."

  self.lerp = "colorgradeLerp"
  self.lerp:nonempty()
  self.lerp.desc = v"{Noun} for the linear interpolation factor, in [0.0, 1.0]."

  result[i] = {
    name = self.name,
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_colorgrade_controller",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
end
return result
