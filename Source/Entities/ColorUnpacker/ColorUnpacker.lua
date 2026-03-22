if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "ColorPacker",
  {
    {"Float", "Int", "HSL", "HSV"},
    parto = {"Sliders", "Counters", "HSL Sliders", "HSV Sliders"},
    output = {"sliders", "counters", "sliders", "sliders"},
    suffixes = {"R, G, B, and A", "R, G, B, and A", "H, S, and L", "H, S, and V"},
    nsuffixes = {"four", "four", "three", "three"},
    tex = {"", "", "_hsl", "_hsv"},
    format = {false, false, true, true},
  },
  {
    {"", "Expression"},
    parfrom = {"Counter", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    cont = {"containing", "yielding"},
    adj = {"set", "truthy"},
    packedColor = {"color", "#color"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Colour Unpacker ({parfrom} to {parto})",
    desc = "Unpacks a counter representing a colour value into several per-component values."
  }

  self[v.noun] = ""
  self[v.noun].desc = v"If present, only update the {output} when the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.packedColor = v.packedColor
  self.packedColor:nonempty()
  self.packedColor.name = "Packed Colour"
  self.packedColor.desc = v[[
    {Noun} {cont} the packed colour that will be unpacked.

    Packing a colour means casting Color.PackedValue to int.
  ]]

  self.unpackedColorPrefix = "unpackedColor"
  self.unpackedColorPrefix:nonempty()
  self.unpackedColorPrefix.name = "Unpacked Colour Prefix"
  self.unpackedColorPrefix.desc = [[
    {suffixes} will be appended to this value to get the {output} names for the {nsuffixes} components of the colour. Alpha is ignored.
  ]]

  if v.format then
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
    texture = v"objects/microlith57/misc/color_unpacker{tex}",
    placements = {self()},
    fieldInformation = self.fieldInformation,
    fieldOrder = self.fieldOrder
  }
end
return result
