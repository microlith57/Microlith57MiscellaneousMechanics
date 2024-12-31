local utils = require("utils")

-- TODO: art

local nonEmptyValidator = function(s)
  return s ~= ""
end

local rgbFieldInformation = {
  packedColor = {validator = nonEmptyValidator}
}

local hslFieldInformation = {
  format = {
    options = {
      "ZeroToOne",
      "Radians",
      "Degrees"
    },
    editable = false
  },
  packedColor = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "packedColor",
  "r", "g", "b", "a",
  "h", "s", "l",
  "alpha",
  "format"
}

return {
  {
    name = "Microlith57Misc/ColorPacker_Float",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_packer",
    placements = {
      {
        name = "colorPacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          r = "1.0",
          g = "1.0",
          b = "1.0",
          a = "1.0",
          alpha = "1.0"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorPacker_Float_Expression",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_packer",
    placements = {
      {
        name = "colorPacker",
        data = {
          expression = "",
          packedColor = "color",
          r = "1.0",
          g = "1.0",
          b = "1.0",
          a = "1.0",
          alpha = "1.0"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorPacker_Int",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_packer",
    placements = {
      {
        name = "colorPacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          r = "255",
          g = "255",
          b = "255",
          a = "255",
          alpha = "1.0"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorPacker_Int_Expression",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_packer",
    placements = {
      {
        name = "colorPacker",
        data = {
          expression = "",
          packedColor = "color",
          r = "255",
          g = "255",
          b = "255",
          a = "255",
          alpha = "1.0"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorPacker_HSL",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_packer",
    placements = {
      {
        name = "colorPacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          h = "0.0",
          s = "1.0",
          l = "1.0",
          alpha = "1.0",
          format = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorPacker_HSL_Expression",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_packer",
    placements = {
      {
        name = "colorPacker",
        data = {
          expression = "",
          packedColor = "color",
          h = ".0",
          s = ".0",
          l = ".0",
          alpha = "1.0",
          format = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslFieldInformation,
    fieldOrder = fieldOrder
  }
}
