local utils = require("utils")

return {
  name = "Microlith57Misc/PlayerFacingFlagController",
  depth = -1000000,
  texture = "objects/microlith57/misc/player_facing_flag_controller",
  placements = {
    {
      name = "playerFacingFlagController",
      data = {
        flagLeft = "playerFacingLeft",
        flagRight = "playerFacingRight",
        persistOnDeath = false
      }
    }
  }
}
