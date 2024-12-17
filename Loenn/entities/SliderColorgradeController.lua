local utils = require("utils")

return {
  {
    name = "Microlith57Misc/SliderColorgradeController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_colorgrade_controller",
    placements = {
      {
        name = "sliderColorgradeController",
        data = {
          flag = "",
          invertFlag = false,
          colorgradeA = "none",
          colorgradeB = "none",
          lerp = "colorgradeLerp"
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderColorgradeController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_colorgrade_controller",
    placements = {
      {
        name = "sliderColorgradeController",
        data = {
          expression = "",
          colorgradeA = "none",
          colorgradeB = "none",
          lerp = "@colorgradeLerp"
        }
      }
    }
  }
}
