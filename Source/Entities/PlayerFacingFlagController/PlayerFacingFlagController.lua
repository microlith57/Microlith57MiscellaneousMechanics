local self = mu.controller {
  "PlayerFacingFlagController",
  name = "Player Facing to Flags Controller",
  desc = "Detects which direction the player is facing, and outputs it as flags."
}

self.flagLeft "playerFacingLeft"
  :nonempty()
  :desc "Flag to set when the player is facing left."
self.flagRight "playerFacingRight"
  :nonempty()
  :desc "Flag to set when the player is facing right."

self.persistOnDeath(false)
  :desc "Whether to keep the flags as they are when the player dies."

return self()
