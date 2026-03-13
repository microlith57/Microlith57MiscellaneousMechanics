if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "ConsumableResource",
  {
    {"Custom", "Stamina", "MaxStamina"},
    res     = {"Custom", "Stamina", "Max Stamina"},
    thing   = {"the resource", "stamina", "max stamina"},
    speed   = {"units/sec", "", "max stamina/sec"},
    default = {"customResource", "stamina", "maxStamina"}
  },
  {
    {"", "Expression"},
    typ = {"Slider", "Expression"},
    adj = {"set", "truthy"}
  }
)

local result = {}
for i, v in ipairs(variants) do
  local bracketed = v.res
  if v.typ == "Expression" then bracketed = bracketed .. "; " .. v.typ end

  local self = mu.entity {
    v.name,
    name = ("Consumable Resource (%s)"):format(bracketed)
  }

  self.resource = v.defaultRes
  self.resource:nonempty()
  self.resource.desc = "Name of this Consumable Resource entity; and also of the slider it uses."

  self.flagPrefix = ""
  self.flagPrefix.desc = 'If present, used as a prefix to generate "Any" / "Full" / "Low" / "Flash" flags.'

  if v.res == "Custom" then
    self.lowThreshold = 20
    self.lowThreshold.desc = "Amount of the resource that's considered low."
    self.maximum = 110
    self.maximum.desc = "Maximum possible amount of the resource."
  elseif v.res == "MaxStamina" then
    self.lowThreshold = 20
    self.lowThreshold.desc = [[
      Amount of max stamina that's considered low.
      Distinct from the low threshold for stamina itself, which is always 20.
    ]]
  end

  self["instantRefill" .. v.typ] = ""
  self["instantRefill" .. v.typ].desc = v[[
    When {adj}, instantly refill {thing} to its maximum.

    May have unintended effects if {adj} for more than a frame.
  ]]
  self["instantDrain" .. v.typ] = ""
  self["instantDrain" .. v.typ].desc = v[[
    When {adj}, instantly set {thing} to 0.

    May have unintended effects if {adj} for more than a frame.
  ]]
  if v.typ == "Flag" then
    self.invertInstantRefillFlag = false
    self.invertInstantDrainFlag = false
  end

  if v.res ~= "Stamina" then
    self.restoreCooldown = 0.1
    self.restoreCooldown.desc = v"Seconds of delay before {thing} can start to refill after being drained."

    self.restoreSpeed = 60
    self.restoreSpeed.desc = v"Speed ({speed}) at which {thing} refills, in [0,∞); or -1 for instant."
  end
  if v.res == "Custom" then
    self.flashRate = 0.05
  end
  if v.res ~= "Stamina" then
    self.useRawDeltaTime = false
    self.useRawDeltaTime.desc = "If true, use real time (unaffected by slowed/sped up time); otherwise use normal game time."
    self.dieWhenConsumed = false
    self.dieWhenConsumed.desc = v"If true, kill the player if {thing} reaches 0."
  end

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.typ == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {self()},
    fieldOrder = self.fieldOrder,
    fieldInformation = self.fieldInformation
  }
end
return result
