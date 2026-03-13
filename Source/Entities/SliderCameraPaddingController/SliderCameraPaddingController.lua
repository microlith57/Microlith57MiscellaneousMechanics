if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "SliderCameraPaddingController",
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
    name = v"Camera Padding Controller ({Noun})",
    desc = "Sets the camera padding (eg. used in the player seeker cutscene) based on a slider value."
  }

  self[v.noun] = ""
  self[v.noun].desc = v"If present, only affect the padding when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.amount = "32.0"
  self.amount:nonempty()
  self.amount.desc = v"{Noun} containing the amount to pad the screen by, in [0.0, ∞). The amount in the vanilla player seeker cutscene is 32.0."

  result[i] = {
    name = self.name,
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_padding_controller",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
end
return result
