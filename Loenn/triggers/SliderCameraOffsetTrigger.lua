local enums = require("consts.celeste_enums")

local fieldInformation = {
  direction = {
    options = enums.trigger_position_modes,
    editable = false
  }
}

local fieldOrder = {
  "x", "y", "width", "height",
  "cameraX", "cameraY",

  "offsetFromSliderX", "offsetFromSliderY",
  "offsetToSliderX", "offsetToSliderY",

  "offsetFromExpressionX", "offsetFromExpressionY",
  "offsetToExpressionX", "offsetToExpressionY",

  "direction", "coarse",

  "enableFlag", "invertFlag",
  "enableExpression"
}

local ignoredFields = {
  "_name", "_id", "originX", "originY",
  "cameraX", "cameraY"
}

return {
  {
    name = "Microlith57Misc/SliderCameraOffsetTrigger",
    category = "camera",
    placements = {
      {
        name = "sliderCameraOffsetTrigger",
        data = {
          cameraX = 0.0,
          cameraY = 0.0,

          offsetFromSliderX = "0.0",
          offsetFromSliderY = "0.0",
          offsetToSliderX = "0.0",
          offsetToSliderY = "0.0",

          direction = "LeftToRight",
          coarse = false,

          enableFlag = "",
          invertFlag = false,
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder,
    ignoredFields = ignoredFields,
    triggerText = "Slider Camera Offset"
  },
  {
    name = "Microlith57Misc/SliderCameraOffsetTrigger_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    category = "camera",
    placements = {
      {
        name = "sliderCameraOffsetTrigger",
        data = {
          cameraX = 0.0,
          cameraY = 0.0,

          offsetFromExpressionX = "0.0",
          offsetFromExpressionY = "0.0",
          offsetToExpressionX = "0.0",
          offsetToExpressionY = "0.0",

          direction = "LeftToRight",
          coarse = false,

          enableExpression = ""
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder,
    ignoredFields = ignoredFields,
    triggerText = "Slider Camera Offset (Expression)"
  }
}