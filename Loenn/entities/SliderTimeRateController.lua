local utils = require("utils")

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
          multiplier = ""
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderTimeRateController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_time_rate_controller",
    placements = {
      {
        name = "sliderTimeRateController",
        data = {
          expression = "",
          multiplier = ""
        }
      }
    }
  }
}
