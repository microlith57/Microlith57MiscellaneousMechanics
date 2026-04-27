local self = mu.controller {
  "PlayerGroundedFlagController",
  name = "Flag if Player Grounded Controller",
  desc = "Detects if the player is on the ground."
}

self.flag "playerGrounded"
  :nonempty()
  :desc "Flag to set when the player is on the ground."
self.invertFlag(false)

return self()
