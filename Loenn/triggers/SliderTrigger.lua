local utils = require("utils")
local celesteEnums = require("consts.celeste_enums")

local fieldInformation = {
  direction = {
    options = celesteEnums.trigger_position_modes,
    editable = false
  }
}

return {
  {
    name = "Microlith57Misc/SliderTrigger",
    placements = {
      {
        name = "sliderTrigger",
        data = {
          width = 16,
          height = 16,
          flag = "",
          invertFlag = false,
          slider = "slider",
          from = 0.0,
          to = 1.0,
          direction = "LeftToRight"
        }
      }
    },
    fieldInformation = fieldInformation
  },
  {
    name = "Microlith57Misc/SliderTrigger_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    placements = {
      {
        name = "sliderTrigger",
        data = {
          width = 16,
          height = 16,
          expression = "",
          slider = "slider",
          from = 0.0,
          to = 1.0,
          direction = "LeftToRight"
        }
      }
    },
    fieldInformation = fieldInformation
  }
}
