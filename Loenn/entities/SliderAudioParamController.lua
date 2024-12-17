local utils = require("utils")

return {
  {
    name = "Microlith57Misc/SliderAudioParamController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_audio_param_controller",
    placements = {
      {
        name = "sliderAudioParamController",
        data = {
          flag = "",
          invertFlag = false,
          param = "fade",
          isAmbience = false,
          value = ""
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderAudioParamController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_audio_param_controller",
    placements = {
      {
        name = "sliderAudioParamController",
        data = {
          expression = "",
          param = "fade",
          isAmbience = false,
          value = ""
        }
      }
    }
  }
}
