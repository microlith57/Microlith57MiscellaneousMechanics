local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local rgbFieldInformation = {
  packedColor = {validator = nonEmptyValidator},
  unpackedColorPrefix = {validator = nonEmptyValidator}
}

local hslvFieldInformation = {
  format = {
    options = {
      "ZeroToOne",
      "Radians",
      "Degrees"
    },
    editable = false
  },
  packedColor = {validator = nonEmptyValidator},
  unpackedColorPrefix = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y",
  "flag", "invertFlag", "expression",
  "packedColor", "unpackedColorPrefix",
  "format"
}

return {
  {
    name = "Microlith57Misc/ColorUnpacker_Float",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          unpackedColorPrefix = "unpackedColor"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_Float_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          expression = "",
          packedColor = "#color",
          unpackedColorPrefix = "unpackedColor"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_Int",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          unpackedColorPrefix = "unpackedColor"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_Int_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          expression = "",
          packedColor = "#color",
          unpackedColorPrefix = "unpackedColor"
        }
      }
    },
    fieldInformation = rgbFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_HSL",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker_hsl",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          unpackedColorPrefix = "unpackedColor",
          format = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslvFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_HSL_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker_hsl",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          expression = "",
          packedColor = "#color",
          unpackedColorPrefix = "unpackedColor",
          format = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslvFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_HSV",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker_hsv",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          flag = "",
          invertFlag = false,
          packedColor = "color",
          unpackedColorPrefix = "unpackedColor",
          format = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslvFieldInformation,
    fieldOrder = fieldOrder
  },
  {
    name = "Microlith57Misc/ColorUnpacker_HSV_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker_hsv",
    placements = {
      {
        name = "colorUnpacker",
        data = {
          expression = "",
          packedColor = "#color",
          unpackedColorPrefix = "unpackedColor",
          format = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslvFieldInformation,
    fieldOrder = fieldOrder
  }
}
