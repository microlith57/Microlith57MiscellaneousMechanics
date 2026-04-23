local variants = mu.variants(
  "SliderColorgradeController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Colourgrade Controller ({Noun})",
    desc = "Interpolates between colourgrades based on a slider value."
  }
  self:_flag_or_expr {v.noun, imperative = "set the colourgrade"}

  local ab = mu.vary {
    ab = {"A", "B"},
    dir = {"from", "to"},
    lerp = {"0.0", "1.0"}
  }
  for _, c in pairs(ab) do
    self["colorgrade" .. c.ab]("none")
      :name(c"Colourgrade {ab}")
      :desc(c"Colourgrade to interpolate {dir} (lerp = {lerp})")
  end

  self.lerp "colorgradeLerp"
    :nonempty()
    :desc(v"{Noun} for the linear interpolation factor, in [0.0, 1.0].")

  result[i] = self()
end
return result
