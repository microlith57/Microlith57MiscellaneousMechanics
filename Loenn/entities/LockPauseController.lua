local utils = require("utils")

local fieldInformation = {
  mode = {
    options = {
      "LockRetry",
      "LockSaveQuit",
      "LockRetryAndSaveQuit",
      "LockPauseMenu"
    },
    editable = false
  }
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "mode", "unlockWhenControllerRemoved"
}

return {
  {
    name = "Microlith57Misc/LockPauseController",
    depth = -1000000,
    texture = "objects/microlith57/misc/lock_pause_controller",
    placements = {
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
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/LockPauseController_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/lock_pause_controller",
    placements = {
      {
        name = "lockRetryController",
        data = {
          expression = "",
          mode = "LockRetry",
          unlockWhenControllerRemoved = true
        }
      },
      {
        name = "lockSaveQuitController",
        data = {
          expression = "",
          mode = "LockSaveQuit",
          unlockWhenControllerRemoved = true
        }
      },
      {
        name = "lockPauseController",
        data = {
          expression = "",
          mode = "LockPauseMenu",
          unlockWhenControllerRemoved = true
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
