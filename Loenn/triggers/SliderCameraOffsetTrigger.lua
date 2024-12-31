local enums = require("consts.celeste_enums")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  direction = {
    options = enums.trigger_position_modes,
    editable = false
  },
  offsetFromX = {validator = nonEmptyValidator},
  offsetFromY = {validator = nonEmptyValidator},
  offsetToX = {validator = nonEmptyValidator},
  offsetToY = {validator = nonEmptyValidator},
}

local fieldOrder = {
  "x", "y", "width", "height",
  "cameraX", "cameraY",

  "offsetFromX", "offsetFromY",
  "offsetToX", "offsetToY",

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

          offsetFromX = "0.0",
          offsetFromY = "0.0",
          offsetToX = "0.0",
          offsetToY = "0.0",

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

          offsetFromX = "0.0",
          offsetFromY = "0.0",
          offsetToX = "0.0",
          offsetToY = "0.0",

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