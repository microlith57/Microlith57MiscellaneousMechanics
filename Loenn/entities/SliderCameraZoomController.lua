local utils = require("utils")

-- TODO: art

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
          focusXSlider = "160",
          focusYSlider = "90",
          amount = "1.0"
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderCameraZoomController_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_zoom_controller",
    placements = {
      {
        name = "sliderCameraZoomController",
        data = {
          expression = "",
          focusXExpression = "160",
          focusYExpression = "90",
          amount = "1.0"
        }
      }
    }
  }
}
