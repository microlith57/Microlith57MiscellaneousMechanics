local utils = require("utils")

local PlayerFacingFlagController = {}

PlayerFacingFlagController.name = "Microlith57Misc/PlayerFacingFlagController"
PlayerFacingFlagController.depth = -1000000
PlayerFacingFlagController.texture = "objects/microlith57/misc/player_facing_flag_controller"
PlayerFacingFlagController.placements = {
    {
        name = "playerFacingFlagController",
        data = {
            flagLeft = "playerFacingLeft",
            flagRight = "playerFacingRight",
            persistOnDeath = false
        }
    }
}

return PlayerFacingFlagController
