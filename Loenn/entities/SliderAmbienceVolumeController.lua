local utils = require("utils")

return {
  {
    name = "Microlith57Misc/SliderAmbienceVolumeController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_ambience_volume_controller",
    placements = {
      {
        name = "sliderAmbienceVolumeController",
        data = {
          flag = "",
          invertFlag = false,
          volume = ""
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderAmbienceVolumeController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_ambience_volume_controller",
    placements = {
      {
        name = "sliderAmbienceVolumeController",
        data = {
          expression = "",
          volume = ""
        }
      }
    }
  }
}
