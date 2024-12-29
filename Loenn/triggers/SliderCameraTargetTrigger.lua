local enums = require("consts.celeste_enums")

-- todo lang

local fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  }
}
local fieldOrder = {
  "x", "y", "width", "height",
  "deleteFlag", "lerpStrength",
  "positionMode", "xOnly", "yOnly",

  "targetSliderX", "targetSliderY",
  "lerpStrengthSliderX", "lerpStrengthSliderY",

  "targetExpressionX", "targetExpressionY",
  "lerpStrengthExpressionX", "lerpStrengthExpressionY",

  "enableFlag", "invertFlag",
  "enableExpression"
}

ignoredFields = {
  "lerpStrength"
}

return {
  {
    name = "Microlith57Misc/SliderCameraTargetTrigger",
    category = "camera",
    nodeLimits = {0, 1},
    placements = {
      {
        name = "sliderCameraTargetTrigger",
        data = {
          deleteFlag = "",
          lerpStrength = 1.0,
          positionMode = "NoEffect",
          xOnly = false,
          yOnly = false,

          targetSliderX = "",
          targetSliderY = "",
          lerpStrengthSliderX = "",
          lerpStrengthSliderY = "",

          enableFlag = "",
          invertFlag = false,
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldInformation,
    ignoredFields = ignoredFields
  },
  {
    name = "Microlith57Misc/SliderCameraTargetTrigger_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    category = "camera",
    nodeLimits = {0, 1},
    placements = {
      {
        name = "sliderCameraTargetTrigger",
        data = {
          deleteFlag = "",
          lerpStrength = 1.0,
          positionMode = "NoEffect",
          xOnly = false,
          yOnly = false,

          targetExpressionX = "",
          targetExpressionY = "",
          lerpStrengthExpressionX = "",
          lerpStrengthExpressionY = "",

          enableExpression = ""
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldInformation,
    ignoredFields = ignoredFields
  }
}