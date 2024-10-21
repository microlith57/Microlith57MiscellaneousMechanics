local utils = require("utils")

local PlayerGroundedFlagController = {}

PlayerGroundedFlagController.name = "Microlith57Misc/PlayerGroundedFlagController"
PlayerGroundedFlagController.depth = -1000000
PlayerGroundedFlagController.texture = "objects/microlith57/misc/player_grounded_flag_controller"
PlayerGroundedFlagController.placements = {
    {
        name = "playerGroundedFlagController",
        data = {
            flag = "playerGrounded",
            invertFlag = false
        }
    }
}

return PlayerGroundedFlagController
