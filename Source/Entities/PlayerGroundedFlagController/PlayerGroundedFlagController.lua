local tex = mu.texture {
  "player_grounded_flag_controller",
  only_editor = true
}

local self = mu.entity {
  "PlayerGroundedFlagController",
  name = "Flag if Player Grounded Controller",
  desc = "Detects if the player is on the ground."
}

self.flag = "playerGrounded"
self.flag:nonempty()
self.flag.desc = "Flag to set when the player is on the ground."
self.invertFlag = false

return {
  name = self.name,
  depth = -1000000,
  texture = tex,
  placements = {self()},
  fieldOrder = self.fieldOrder,
  fieldInformation = self.fieldInformation
}
