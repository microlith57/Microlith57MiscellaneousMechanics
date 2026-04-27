local enums = require("consts.celeste_enums")

local variants = mu.variants(
  "SliderCameraOffsetTrigger",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local name = v"Camera Target ({Float})"
  local self = mu.trigger {
    v.name,
    name = name,
    desc = v"Sets the camera target based on {float} values."
  }

  result[i] = self {

  }
end
return result


local fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  },
  snapMode = {
    options = {
      "NeverSnap",
      "SnapWhenInitiallyEnabled",
      "AlwaysSnap",
      "AlwaysSnapIgnoringRoomBounds"
    },
    editable = false
  },
}

local fieldOrder = {
  "x", "y", "width", "height",
  "deleteFlag",
  "positionMode", "xOnly", "yOnly",

  "targetX", "targetY",
  "lerpStrengthX", "lerpStrengthY",
  "snapMode",

  "enableFlag", "invertFlag",
  "enableExpression"
}

ignoredFields = {
  "_name", "_id", "originX", "originY"
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
          positionMode = "NoEffect",
          xOnly = false,
          yOnly = false,

          targetX = "targetX",
          targetY = "targetY",
          lerpStrengthX = "1.0",
          lerpStrengthY = "1.0",
          snapMode = "NeverSnap",

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
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    category = "camera",
    nodeLimits = {0, 1},
    placements = {
      {
        name = "sliderCameraTargetTrigger",
        data = {
          deleteFlag = "",
          positionMode = "NoEffect",
          xOnly = false,
          yOnly = false,

          targetX = "@targetX",
          targetY = "@targetY",
          lerpStrengthX = "1.0",
          lerpStrengthY = "1.0",
          snapMode = "NeverSnap",

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
