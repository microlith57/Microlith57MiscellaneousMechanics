local utils = require("utils")

-- TODO: art; int

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
          unpackedColorPrefix = "unpackedColor",
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
          unpackedColorPrefix = "unpackedColor",
        }
      }
    }
  }
}
