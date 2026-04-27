local variants = mu.variants(
  "SliderColorgradeController",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Colourgrade Controller ({Float})",
    desc = v"Interpolates between colourgrades based on the value of {a float}."
  }
  self:_flag_or_expr {v.noun, imperative = "set the colourgrade"}

  local ab = mu.vary {
    ab = {"A", "B"},
    dir = {"from", "to"},
    lerp = {"0.0", "1.0"}
  }
  for _, c in pairs(ab) do
    self["colorgrade" .. c.ab]
      :default "none"
      :name(c"Colourgrade {ab}")
      :desc(c"Colourgrade to interpolate {dir} (lerp = {lerp})")
  end

  self.lerp "colorgradeLerp"
    :nonempty()
    :desc(v"{Float} {containing} the linear interpolation factor, in [0.0, 1.0].")

  result[i] = self()
end
return result
