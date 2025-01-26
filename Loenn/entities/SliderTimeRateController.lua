local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  multiplier = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "multiplier"
}

return {
  {
    name = "Microlith57Misc/SliderTimeRateController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_time_rate_controller",
    placements = {
      {
        name = "sliderTimeRateController",
        data = {
          flag = "",
          invertFlag = false,
          multiplier = "1.0"
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderTimeRateController_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_time_rate_controller",
    placements = {
      {
        name = "sliderTimeRateController",
        data = {
          expression = "",
          multiplier = "1.0"
        }
      }
    }
  }
}
