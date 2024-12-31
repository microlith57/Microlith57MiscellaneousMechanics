local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  slider = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "enabledFlag", "invertEnabledFlag", "enabledExpression",
  "activeFlag", "invertActiveFlag", "activeExpression",
  "slider",
  "consumptionResourceName", "consumptionRate", "unfocusWhenResourceLow",
  "fadeDuration", "useRawDeltaTime"
}

return {
  {
    name = "Microlith57Misc/FocusController",
    depth = -1000000,
    texture = "objects/microlith57/misc/focus_controller",
    placements = {
      {
        name = "focusController",
        data = {
          enabledFlag = "",
          invertEnabledFlag = false,
          activeFlag = "tryingToFocus",
          invertActiveFlag = "",
          slider = "focus",
          consumptionResourceName = "",
          consumptionRate = 12.0,
          unfocusWhenResourceLow = true,
          fadeDuration = 1.0,
          useRawDeltaTime = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/FocusController_Button",
    depth = -1000000,
    texture = "objects/microlith57/misc/focus_controller",
    placements = {
      {
        name = "focusController",
        data = {
          enabledFlag = "",
          invertEnabledFlag = false,
          slider = "focus",
          consumptionResourceName = "",
          consumptionRate = 12.0,
          unfocusWhenResourceLow = true,
          fadeDuration = 1.0,
          useRawDeltaTime = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/FocusController_Expression",
    depth = -1000000,
    texture = "objects/microlith57/misc/focus_controller",
    placements = {
      {
        name = "focusController",
        data = {
          enabledExpression = "",
          activeExpression = "$input.grab",
          slider = "focus",
          consumptionResourceName = "",
          consumptionRate = 12.0,
          unfocusWhenResourceLow = true,
          fadeDuration = 1.0,
          useRawDeltaTime = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
