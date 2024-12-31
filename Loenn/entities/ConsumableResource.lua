local utils = require("utils")

-- todo: art

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
