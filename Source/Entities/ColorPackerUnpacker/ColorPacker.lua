local par = {
  Float = {"Sliders", "Float Expressions"},
  Int = {"Counters", "Int Expressions"},
  HSL = {"HSL Sliders", "HSL Expressions"},
  HSV = {"HSV Sliders", "HSV Expressions"},
}

local rgba = mu.vary {
  col = {"r", "g", "b", "a"},
  cname = {"red", "green", "blue", "premultiplied alpha"},
  Cname = {"Red", "Green", "Blue", "Premult Alpha"},
}
local hsl = mu.vary {col = {"h", "s", "l"}, cname = {"hue", "saturation", "lightness"}}
local hsv = mu.vary {col = {"h", "s", "v"}, cname = {"hue", "saturation", "value"}}

local variants = mu.variants(
  "ColorPacker",
  {
    {"Float", "Int", "HSL", "HSV"},
    channels = {rgba, rgba, hsl, hsv},
    tex = {"", "", "_HSL", "_HSV"},
  },
  mu.var_expr {
    parj = {1, 2},
  }
)

local result = {}
for i, v in ipairs(variants) do
  v.par = par[v[1][1]][v.parj]

  local self = mu.controller {
    v.name,
    name = v"Colour Packer ({par} to Counter)",
    desc = "Packs per-component values into a single counter representing a colour value.",
    texture = v"ColorPacker{tex}",
  }
  self:_flag_or_expr {v.bool, imperative = "update the counter"}

  self.packedColor "color"
    :nonempty()
    :name "Packed Colour"
    :desc [[
      Counter that will be set to the packed colour.

      Packing a colour means casting Color.PackedValue to int.
    ]]

  if v[1][1] == "Int" then
    v.source = v.int
    v.l = "0" v.u = "255"
  else
    v.source = v.float
    v.l = "0.0" v.u = "1.0"
  end
  for _, c in ipairs(v.channels) do
    c(v)
    c.range = c.col == "h" and " (see Angle Format)" or v", in {[l, u]}"

    self[c.col]
      :default(c.col == "h" and "0.0" or v.h)
      :name(c.Cname)
      :desc(c"{Source} for the {cname} component{range}.")
  end

  self.alpha = "1.0"
  if v[1][1] == "Float" or v[1][1] == "Int" then
    self.alpha.desc = v[[
      {Float} for an additional alpha multiplier.

      If you don't know what premultiplied alpha means, use this instead.
    ]]
  else
    self.alpha.desc = v"{Float} for the alpha multiplier."
  end

  if v.channels[1].col == "h" then
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
