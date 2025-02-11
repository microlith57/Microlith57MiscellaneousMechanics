local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  tag = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "tag",
  "positionX", "positionY",
  "scrollX", "scrollY",
  "speedX", "speedY",
  "packedColor",
  "alphaMultiplier"
}

return {

  {
    name = "Microlith57Misc/SliderStylegroundController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_styleground_controller",
    placements = {
      {
        name = "sliderStylegroundController",
        data = {
          flag = "",
          invertFlag = false,
          tag = "",
          positionX = "",
          positionY = "",
          scrollX = "",
          scrollY = "",
          speedX = "",
          speedY = "",
          packedColor = "",
          alphaMultiplier = ""
        }
      }
    }
  },
  {
    name = "Microlith57Misc/SliderStylegroundController_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_styleground_controller",
    placements = {
      {
        name = "sliderStylegroundController",
        data = {
          expression = "",
          tag = "",
          positionX = "",
          positionY = "",
          scrollX = "",
          scrollY = "",
          speedX = "",
          speedY = "",
          packedColor = "",
          alphaMultiplier = ""
        }
      }
    }
  }
}
