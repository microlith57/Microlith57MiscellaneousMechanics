local utils = require("utils")

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression"
}

return {
  {
    name = "Microlith57Misc/FreezeTimeActiveController",
    depth = -1000000,
    texture = "objects/microlith57/misc/freeze_time_active_controller",
    placements = {
      {
        name = "freezeTimeActiveController",
        data = {
          flag = "freezeTimeActive",
          invertFlag = false
        }
      }
    },
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/FreezeTimeActiveController_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/freeze_time_active_controller",
    placements = {
      {
        name = "freezeTimeActiveController",
        data = {
          expression = "freezeTimeActive"
        }
      }
    },
    fieldOrder = fieldOrder
  }
}
