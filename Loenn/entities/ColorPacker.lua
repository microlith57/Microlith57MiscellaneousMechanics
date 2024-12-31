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
          r = "1",
          g = "1",
          b = "1",
          a = "1",
          alpha = "1"
        }
      }
    }
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
          r = "1",
          g = "1",
          b = "1",
          a = "1",
          alpha = "1"
        }
      }
    }
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
          alpha = "1"
        }
      }
    }
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
          alpha = "1"
        }
      }
    }
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
          h = "0",
          s = "1",
          l = "1",
          alpha = "1",
          angleFormat = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslFieldInformation
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
          h = "0",
          s = "1",
          l = "1",
          alpha = "1",
          angleFormat = "ZeroToOne"
        }
      }
    },
    fieldInformation = hslFieldInformation
  }
}
