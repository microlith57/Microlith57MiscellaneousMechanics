local utils = require("utils")

-- TODO: art

local hslFieldInformation = {
  format = {
    options = {
      "ZeroToOne",
      "Radians",
      "Degrees"
    },
    editable = false
  }
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
    }
  },
  {
    name = "Microlith57Misc/ColorUnpacker_Float_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
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
    }
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
    }
  },
  {
    name = "Microlith57Misc/ColorUnpacker_Int_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
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
    }
  },
  {
    name = "Microlith57Misc/ColorUnpacker_HSL",
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker",
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
    fieldInformation = hslFieldInformation
  },
  {
    name = "Microlith57Misc/ColorUnpacker_HSL_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    depth = -1000000,
    texture = "objects/microlith57/misc/color_unpacker",
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
    fieldInformation = hslFieldInformation
  }
}
