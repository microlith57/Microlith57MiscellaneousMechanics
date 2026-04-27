local variants = mu.variants(
  "SliderStylegroundController",
  mu.var_expr()
)

local things = {"position", "scroll", "speed"}
local coords = mu.vary {a = {"x", "y"}}

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Styleground Controller ({Float})",
    desc = "Sets properties of stylegrounds based on {float} values."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the stylegrounds"}

  self.tag ""
    :nonempty()
    :desc "Affect stylegrounds with this tag."

  for _, t in ipairs(things) do
    for _, c in ipairs(coords) do
      c(v); c.thing = t

      self[t .. c.A]("")
        :desc(c"{Float} to set the {A} {thing} to, or empty to leave it unchanged.")
    end
  end

  self.packedColor ""
    :name "Packed Colour"
    :desc(v[[
      {Float} to use to set the colour, or empty to leave it unchanged.
      Must be packed eg. with the Colour Packer entity.
    ]])

  self.alphaMultiplier ""
    :desc(v"{Float} to set the alpha multiplier to, or empty to leave it unchanged.")

  result[i] = self()
end
return result
