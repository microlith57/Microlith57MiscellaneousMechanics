local variants = mu.variants(
  "SliderAudioParamController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Audio Param Controller ({Noun})",
    desc = "Sets an audio (music or ambience) param based on a slider value."
  }
  self:_flag_or_expr {v.noun, imperative = "set the param"}

  self.isAmbience(false)
    :desc "If checked, set an ambience param; otherwise, set a music param."
  self.param("")
    :desc "Name of the param to set."
  self.value "0.0"
    :nonempty()
    :desc(v"{Noun} to set the param to.")

  result[i] = self {
    {
      "music",
      name = v"Music Param Controller ({Noun})",
      desc = "Sets a music param based on a slider value.",
      data = {isAmbience = false},
    },
    {
      "ambience",
      name = v"Ambience Param Controller ({Noun})",
      desc = "Sets an ambience param based on a slider value.",
      data = {isAmbience = true},
    }
  }
end
return result
