local variants = mu.variants(
  "ColorUnpacker",
  {
    {"Float", "Int", "HSL", "HSV"},
    parto = {"Sliders", "Counters", "HSL Sliders", "HSV Sliders"},
    output = {"sliders", "counters", "sliders", "sliders"},
    suffixes = {"R, G, B, and A", "R, G, B, and A", "H, S, and L", "H, S, and V"},
    nsuffixes = {"four", "four", "three", "three"},
    tex = {"", "", "_HSL", "_HSV"},
    format = {false, false, true, true},
  },
  {
    {"", "Expression"},
    parfrom = {"Counter", "Expression"},
    noun = {"flag", "expression"},
    cont = {"containing", "yielding"},
    adj = {"set", "truthy"},
    packedColor = {"color", "#color"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Colour Unpacker ({parfrom} to {parto})",
    desc = "Unpacks a counter representing a colour value into several per-component values.",
    texture = v"ColorUnpacker{tex}",
  }
  self:_flag_or_expr {v.noun, imperative = v"update the {output}"}

  self.packedColor(v.packedColor)
    :nonempty()
    :name "Packed Colour"
    :desc(v[[
      {parfrom} {cont} the packed colour that will be unpacked.

      Packing a colour means casting Color.PackedValue to int.
    ]])

  self.unpackedColorPrefix "unpackedColor"
    :nonempty()
    :name "Unpacked Colour Prefix"
    :desc(v[[
      {suffixes} will be appended to this value to get the {output} names for the {nsuffixes} components of the colour. Alpha is ignored.
    ]])

  if v.format then
    self.format "ZeroToOne"
      :info {
        options = {"ZeroToOne", "Radians", "Degrees"},
        editable = false
      }
      :name "Angle Format"
      :desc "Format to use for the hue component."
  end

  result[i] = self()
end
return result
