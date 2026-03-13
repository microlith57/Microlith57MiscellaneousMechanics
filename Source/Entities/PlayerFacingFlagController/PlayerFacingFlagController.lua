if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local self = mu.entity {
  "PlayerFacingFlagController",
  name = "Player Facing to Flags Controller",
  desc = "Detects which direction the player is facing, and outputs it as flags."
}

self.flagLeft = "playerFacingLeft"
self.flagLeft:nonempty()
self.flagLeft.desc = "Flag to set when the player is facing left."
self.flagRight = "playerFacingRight"
self.flagRight:nonempty()
self.flagRight.desc = "Flag to set when the player is facing right."

self.persistOnDeath = false
self.persistOnDeath.desc = "Whether to keep the flags as they are when the player dies."

return {
  name = self.name,
  depth = -1000000,
  texture = "objects/microlith57/misc/player_facing_flag_controller",
  placements = {self()},
  fieldOrder = self.fieldOrder,
  fieldInformation = self.fieldInformation
}
