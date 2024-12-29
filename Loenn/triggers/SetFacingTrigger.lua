local utils = require("utils")

local fieldInformation = {
  direction = {
    options = {
      "Left",
      "Right"
    },
    editable = false
  }
}

return {
  {
    name = "Microlith57Misc/SetFacingTrigger",
    placements = {
      {
        name = "setFacingTrigger",
        data = {
          width = 16,
          height = 16,
          direction = "Left",
          flag = "",
          invertIfUnset = false,
          continuous = true
        }
      }
    },
    fieldInformation = fieldInformation
  },
  {
    name = "Microlith57Misc/SetFacingTrigger_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    placements = {
      {
        name = "setFacingTrigger",
        data = {
          width = 16,
          height = 16,
          direction = "Left",
          expression = "",
          continuous = true
        }
      }
    },
    fieldInformation = fieldInformation
  }
}
