local utils = require("utils")

local LevelTimerToCounterController = {}

LevelTimerToCounterController.name = "Microlith57Misc/LevelTimerToCounterController"
LevelTimerToCounterController.depth = -1000000
LevelTimerToCounterController.texture = "objects/microlith57/misc/level_timer_to_counter_controller"
LevelTimerToCounterController.placements = {
    {
        name = "levelTimerSecondsToCounterController",
        data = {
            flag = "",
            invertFlag = false,
            counter = "levelTimer",
            wrapMode = "NoWrapping",
            precisionMode = "Seconds"
        }
    },
    {
        name = "levelTimerFramesToCounterController",
        data = {
            flag = "",
            invertFlag = false,
            counter = "levelTimer",
            wrapMode = "NoWrapping",
            precisionMode = "FramesProbably"
        }
    },
    {
        name = "levelTimerMillisecondsToCounterController",
        data = {
            flag = "",
            invertFlag = false,
            counter = "levelTimer",
            wrapMode = "NoWrapping",
            precisionMode = "Milliseconds"
        }
    }
}
LevelTimerToCounterController.fieldInformation = {
    wrapMode = {
        options = {
            "NoWrapping",
            "Positive",
            "FullRange"
        }
    },
    precisionMode = {
        options = {
            "Seconds",
            "FramesProbably",
            "Milliseconds"
        }
    }
}

return LevelTimerToCounterController
