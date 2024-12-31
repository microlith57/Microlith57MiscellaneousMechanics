local utils = require("utils")

return {
  name = "Microlith57Misc/PlayerGroundedFlagController",
  depth = -1000000,
  texture = "objects/microlith57/misc/player_grounded_flag_controller",
  placements = {
    {
      name = "playerGroundedFlagController",
      data = {
        flag = "playerGrounded",
        invertFlag = false
      }
    }
  }
}
