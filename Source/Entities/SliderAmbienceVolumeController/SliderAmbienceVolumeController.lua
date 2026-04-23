local variants = mu.variants(
  "SliderAmbienceVolumeController",
  {
    {"", "Expression"},
    typ  = {"Slider", "Expression"},
    noun = {"flag", "expression"},
    adj  = {"set", "truthy"}
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Ambience Volume Controller ({typ})"
  }
  self:_flag_or_expr {v.noun, imperative = "set the volume"}

  self.volume "1.0"
    :nonempty()
    :desc(v"{typ} to set the volume to.")

  result[i] = self()
end
return result
