local variants = mu.variants(
  "SliderStylegroundController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
  }
)

local things = {"position", "scroll", "speed"}
local coords = mu.vary {a = {"x", "y"}}

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Styleground Controller ({Noun})",
    desc = "Sets properties on a styleground on a slider value."
  }
  self:_flag_or_expr {v.noun, imperative = "affect the stylegrounds"}

  self.tag ""
    :nonempty()
    :desc "Affect stylegrounds with this tag."

  for _, t in ipairs(things) do
    for _, c in ipairs(coords) do
      c(v); c.thing = t

      self[t .. c.A]("")
        :desc(c"{Noun} to set the {A} {thing} to, or empty to leave it unchanged.")
    end
  end

  self.packedColor ""
    :name "Packed Colour"
    :desc(v[[
      {Noun} to use to set the colour, or empty to leave it unchanged.
      Must be packed eg. with the Colour Packer entity.
    ]])

  self.alphaMultiplier ""
    :desc(v"{Noun} to set the alpha multiplier by, or empty to leave it unchanged.")

  result[i] = self()
end
return result
