if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local par = {
  Float = {"Sliders", "Float Expressions"},
  Int = {"Counters", "Int Expressions"},
  HSL = {"HSL Sliders", "HSL Expressions"},
  HSV = {"HSV Sliders", "HSV Expressions"},
}

local rgba = mu.vary {
  col = {"r", "g", "b", "a"},
  cName = {"Red", "Green", "Blue", "Premult Alpha"},
  cname = {"red", "green", "blue", "premultiplied alpha"},
}

local hsl = mu.vary {
  col = {"h", "s", "l"},
  cName = {"Hue", "Saturation", "Lightness"},
  cname = {"hue", "saturation", "lightness"},
}

local hsv = mu.vary {
  col = {"h", "s", "v"},
  cName = {"Hue", "Saturation", "Value"},
  cname = {"hue", "saturation", "value"},
}

local variants = mu.variants(
  "ColorPacker",
  {
    {"Float", "Int", "HSL", "HSV"},
    channels = {rgba, rgba, hsl, hsv},
    tex = {"", "", "_hsl", "_hsv"},
  },
  {
    {"", "Expression"},
    parj = {1, 2},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  v.par = par[v[1][1]][v.parj]

  local self = mu.entity {
    v.name,
    name = v"Colour Packer ({par} to Counter)",
    desc = "Packs per-component values into a single counter representing a colour value."
  }

  self[v.noun] = ""
  self[v.noun].desc = v"If present, only update the counter when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.packedColor = "color"
  self.packedColor:nonempty()
  self.packedColor.name = "Packed Colour"
  self.packedColor.desc = [[
    Counter that will be set to the packed colour.

    Packing a colour means casting Color.PackedValue to int.
  ]]

  if v[1][1] == "Int" then
    v.l = "0" v.u = "255"
  else
    v.l = "0.0" v.u = "1.0"
  end
  for _, c in ipairs(channels) do
    c(v)
    c.range = c.col == "h" and " (see Angle Format)" or v", in [{l}, {u}]"
    
    self[c.col] = c.col == "h" and "0.0" or v.h
    self[c.col].name = c.cName
    self[c.col].desc = c"{Noun} for the {cname} component{range}."
  end

  self.alpha = "1.0"
  if v[1][1] == "Float" or v[1][1] == "Int" then
    self.alpha.desc = v[[
      {Noun} for an additional alpha multiplier.

      If you don't know what premultiplied alpha means, use this instead.
    ]]
  else
    self.alpha.desc = v"{Noun} for the alpha multiplier."
  end

  if channels[1].col == "h" then
    self.format = "ZeroToOne"
    self.format.info = {
        options = {"ZeroToOne", "Radians", "Degrees"},
        editable = false
    }
    self.format.name = "Angle Format"
    self.format.desc = "Format to use for the hue component."
  end

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.Noun == "Expression"},
    depth = -1000000,
    texture = v"objects/microlith57/misc/color_packer{tex}",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
end
return result
