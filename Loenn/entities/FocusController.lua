local utils = require("utils")

-- todo: lang, art

return {
  {
    name = "Microlith57Misc/FocusController",
    depth = -1000000,
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
    }
  },
  {
    name = "Microlith57Misc/FocusController_Button",
    depth = -1000000,
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
    }
  },
  {
    name = "Microlith57Misc/FocusController_Expression",
    depth = -1000000,
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
    }
  }
}
