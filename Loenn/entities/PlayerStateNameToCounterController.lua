local utils = require("utils")

-- todo: art

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  stateName = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "stateName", "counter",
  "inStateFlag", "invertFlag"
}

return {
  {
    name = "Microlith57Misc/PlayerStateNameToCounterController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_sound_source",
    placements = {
      {
        name = "playerStateNameToCounterController",
        data = {
          stateName = "StNormal",
          counter = "stNormal",
          inStateFlag = "stNormal",
          invertFlag = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
