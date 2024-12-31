local enums = require("consts.celeste_enums")

local nonEmptyValidator = function(s)
  return s ~= ""
end

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

  "targetX", "targetY",
  "lerpStrengthX", "lerpStrengthY",

  "enableFlag", "invertFlag",
  "enableExpression"
}

ignoredFields = {
  "_name", "_id", "originX", "originY",
  "lerpStrengthX", "lerpStrengthY"
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
          lerpStrengthX = 1.0,
          lerpStrengthY = 1.0,
          positionMode = "NoEffect",
          xOnly = false,
          yOnly = false,

          targetSliderX = "targetX",
          targetSliderY = "targetY",
          lerpStrengthSliderX = "1.0",
          lerpStrengthSliderY = "1.0",

          enableFlag = "",
          invertFlag = false,
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder,
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
          lerpStrengthX = 1.0,
          lerpStrengthY = 1.0,
          positionMode = "NoEffect",
          xOnly = false,
          yOnly = false,

          targetExpressionX = "@targetX",
          targetExpressionY = "@targetX",
          lerpStrengthExpressionX = "1.0",
          lerpStrengthExpressionY = "1.0",

          enableExpression = ""
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder,
    ignoredFields = ignoredFields,
    triggerText = "Slider Camera Target (Expression)"
  }
}