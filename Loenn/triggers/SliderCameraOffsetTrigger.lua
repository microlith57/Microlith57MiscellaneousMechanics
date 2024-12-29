local enums = require("consts.celeste_enums")

-- todo lang

local fieldOrder = {
  "x", "y", "width", "height",
  "cameraX", "cameraY",

  "offsetFromSliderX", "offsetFromSliderY",
  "offsetToSliderX", "offsetToSliderY",

  "offsetFromExpressionX", "offsetFromExpressionY",
  "offsetToExpressionX", "offsetToExpressionY",

  "coarse",

  "enableFlag", "invertFlag",
  "enableExpression"
}

local ignoredFields = {
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

          coarse = false,

          enableFlag = "",
          invertFlag = false,
        }
      }
    },
    fieldOrder = fieldOrder,
    ignoredFields = ignoredFields
  },
  {
    name = "Microlith57Misc/SliderCameraOffsetTrigger_Expression",
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

          coarse = false,

          enableExpression = ""
        }
      }
    },
    fieldOrder = fieldOrder,
    ignoredFields = ignoredFields
  }
}