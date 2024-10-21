local utils = require("utils")

local LockPauseController = {}

LockPauseController.name = "Microlith57Misc/LockPauseController"
LockPauseController.depth = -1000000
LockPauseController.texture = "objects/microlith57/misc/lock_pause_controller"
LockPauseController.placements = {
    {
        name = "lockRetryController",
        data = {
            flag = "",
            invertFlag = false,
            mode = "LockRetry",
            unlockWhenControllerRemoved = true
        }
    },
    {
        name = "lockSaveQuitController",
        data = {
            flag = "",
            invertFlag = false,
            mode = "LockSaveQuit",
            unlockWhenControllerRemoved = true
        }
    },
    {
        name = "lockPauseController",
        data = {
            flag = "",
            invertFlag = false,
            mode = "LockPauseMenu",
            unlockWhenControllerRemoved = true
        }
    },
}
LockPauseController.fieldInformation = {
    mode = {
        options = {
            "LockRetry",
            "LockSaveQuit",
            "LockRetryAndSaveQuit",
            "LockPauseMenu"
        }
    }
}

return LockPauseController
