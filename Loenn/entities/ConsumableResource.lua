local utils = require("utils")

-- todo: art

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
          useRawDeltaTime = false,
          dieWhenConsumed = false
        }
      }
    }
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
    }
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
    }
  }
}
