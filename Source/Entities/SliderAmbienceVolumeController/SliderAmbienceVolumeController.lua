local variants = mu.variants(
  "SliderAmbienceVolumeController",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Ambience Volume Controller ({Float})"
  }
  self:_flag_or_expr {v.bool, imperative = "set the volume"}

  self.volume "1.0"
    :nonempty()
    :desc(v"{Float} to set the volume to.")

  result[i] = self()
end
return result
