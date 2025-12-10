local utils = require("utils")

local fieldInformation = {
  color = {
    fieldType = "color"
  }
}

return {
  name = "Microlith57Misc/RecorderTerminal",
  depth = 2000,
  texture = "objects/microlith57/misc/terminal",
  justification = {0.5, 1.0},
  placements = {
    name = "normal",
    data = {
      color = "ac3232",
      maxDuration = 60
    }
  },
  fieldInformation = fieldInformation
}
