local utils = require("utils")

local SetFacingTrigger = {}

SetFacingTrigger.name = "Microlith57Misc/SetFacingTrigger"
SetFacingTrigger.placements = {
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
}
SetFacingTrigger.fieldInformation = {
  direction = {
    options = {
      "Left",
      "Right"
    }
  }
}

return SetFacingTrigger
