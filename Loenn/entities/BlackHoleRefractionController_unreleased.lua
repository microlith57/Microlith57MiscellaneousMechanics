local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
}

local fieldOrder = {
  "x", "y",
  "enabled", "invertEnabled",
}

return {
  {
    name = "Microlith57Misc/BlackHoleRefractionController",
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_camera_padding_controller",
    placements = {
      {
        name = "blackHoleRefractionController",
        data = {
          enabled = "enableBlackHoleRefraction",
          invertEnabled = false
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder
  }
}
