if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

---

local name = "ConsumableResource"
local placement = "consumableResource"
local variants = mu.variants(
  name,
  {
    {"Custom", "Stamina", "MaxStamina"},
    ---
    {"Custom", "Stamina", "Max Stamina"}, -- res
    {"the resource", "stamina", "max stamina"}, -- the
    {"units/sec", "", "max stamina/sec"}, -- speed
    ---
    {"customResource", "stamina", "maxStamina"}, -- defaultRes
  },
  {
    {"", "Expression"},
    ---
    {"Slider", "Expression"}, -- typ
    {"set", "truthy"}, -- adj
  }
)

if mu.preprocess then
  for _, v in ipairs(variants) do
    local res   = v[1][2]
    local the   = v[1][3]
    local speed = v[1][4]

    local typ  = v[2][2]
    local adj  = v[2][3]

    local bracketed = res
    if typ == "Expression" then bracketed = bracketed .. "; " .. typ end

    local ent = mu.preprocess.lang.entities[v.name]
    ent.placements.name[placement] = ("Consumable Resource (%s)"):format(bracketed)
    desc = ent.attributes.description
    desc.resource = "Name of this Consumable Resource entity; and also of the slider it uses."
    desc.flagPrefix = 'If present, used as a prefix to generate "Any" / "Full" / "Low" / "Flash" flags.'

    desc["instantRefill" .. typ] = ([[
      When %s, instantly refill %s to its maximum.

      May have unintended effects if %s for more than a frame.
    ]]):format(adj, the, adj)
    desc["instantDrain" .. typ] = ([[
      When %s, instantly set %s to 0.

      May have unintended effects if %s for more than a frame.
    ]]):format(adj, the, adj)

    if res ~= "Stamina" then
      desc.restoreCooldown = ("Seconds of delay before %s can start to refill after being drained."):format(the)
      desc.restoreSpeed = ("Speed (%s) at which %s refills, in [0,∞); or -1 for instant."):format(speed, the)
      desc.useRawDeltaTime = "If true, use real time (unaffected by slowed/sped up time); otherwise use normal game time."
      desc.dieWhenConsumed = ("If true, kill the player if %s reaches 0."):format(the)
    end

    if res == "Custom" then
      desc.lowThreshold = "Amount of the resource that's considered low."
      desc.maximum = "Maximum possible amount of the resource."
    elseif res == "Max Stamina" then
      desc.lowThreshold = [[
        Amount of max stamina that's considered low.
        Distinct from the low threshold for stamina itself, which is always 20.
      ]]
    end
  end
  return
end

---

local result = {}
for i, v in ipairs(variants) do
  local res = v[1][2]
  local defaultRes = v[1][5]
  
  local typ = v[2][2]

  local b = mu.builder():xy()

  b.resource = defaultRes
  b:nonempty(resource)

  b.flagPrefix = ""
  b["instantRefill" .. typ] = ""
  b["instantDrain" .. typ] = ""

  if typ == "Flag" then
    b.invertInstantRefillFlag = false
    b.invertInstantDrainFlag = false
  end

  result[i] = {
    name = v.name,
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      b.placement(placement)
    },
    fieldOrder = b.fieldOrder,
    fieldInformation = b.fieldInformation
  }
end
return result

---

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  resource = {validator = nonEmptyValidator},
  unpackedColorPrefix = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "resource", "flagPrefix",
  "lowThreshold", "maximum",
  "instantRefillFlag", "instantDrainFlag", "instantRefillExpression", "instantDrainExpression",
  "restoreCooldown", "restoreSpeed", "flashRate", "useRawDeltaTime",
  "dieWhenConsumed"
}

return {
  {
    name = "Microlith57Misc/ConsumableResource_Custom",
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      {
        name = "consumableResource",
        data = {
          resource = "customResource",
          flagPrefix = "",
          lowThreshold = 20,
          maximum = 110,
          instantRefillFlag = "",
          instantDrainFlag = "",
          restoreCooldown = 0.1,
          restoreSpeed = 60,
          flashRate = 0.05,
          useRawDeltaTime = false,
          dieWhenConsumed = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ConsumableResource_Custom_Expresssion",
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      {
        name = "consumableResource",
        data = {
          resource = "customResource",
          flagPrefix = "",
          lowThreshold = 20,
          maximum = 110,
          instantRefillExpression = "",
          instantDrainExpression = "",
          restoreCooldown = 0.1,
          restoreSpeed = 60,
          useRawDeltaTime = false,
          dieWhenConsumed = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ConsumableResource_Stamina",
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      {
        name = "consumableResource",
        data = {
          resource = "stamina",
          flagPrefix = "",
          instantRefillFlag = "",
          instantDrainFlag = ""
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ConsumableResource_Stamina_Expresssion",
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      {
        name = "consumableResource",
        data = {
          resource = "stamina",
          flagPrefix = "",
          instantRefillExpression = "",
          instantDrainExpression = ""
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ConsumableResource_MaxStamina",
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      {
        name = "consumableResource",
        data = {
          resource = "maxStamina",
          flagPrefix = "",
          lowThreshold = 20,
          instantRefillFlag = "",
          instantDrainFlag = "",
          restoreCooldown = 0.1,
          restoreSpeed = 60,
          useRawDeltaTime = false,
          dieWhenConsumed = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ConsumableResource_MaxStamina_Expresssion",
    depth = -1000000,
    texture = "objects/microlith57/misc/consumable_resource",
    placements = {
      {
        name = "consumableResource",
        data = {
          resource = "maxStamina",
          flagPrefix = "",
          lowThreshold = 20,
          instantRefillExpression = "",
          instantDrainExpression = "",
          restoreCooldown = 0.1,
          restoreSpeed = 60,
          useRawDeltaTime = false,
          dieWhenConsumed = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
