local utils = require("utils")

local FreezeTimeActiveController = {}

FreezeTimeActiveController.name = "Microlith57Misc/FreezeTimeActiveController"
FreezeTimeActiveController.depth = -1000000
FreezeTimeActiveController.texture = "objects/microlith57/misc/freeze_time_active_controller"
FreezeTimeActiveController.placements = {
    {
        name = "freezeTimeActiveController",
        data = {
            flag = "freezeTimeActive",
            invertFlag = false
        }
    }
}

return FreezeTimeActiveController
