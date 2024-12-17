local utils = require("utils")

-- TODO: lang, art

return {
  {
    name = "Microlith57Misc/SliderCameraZoomController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_zoom_controller",
    placements = {
      {
        name = "sliderCameraZoomController",
        data = {
          flag = "",
          invertFlag = false,
          focusXSlider = "",
          focusYSlider = "",
          amount = ""
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderCameraZoomController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_time_rate_controller",
    placements = {
      {
        name = "sliderCameraZoomController",
        data = {
          expression = "",
          focusXExpression = "",
          focusYExpression = "",
          amount = ""
        }
      }
    }
  }
}
