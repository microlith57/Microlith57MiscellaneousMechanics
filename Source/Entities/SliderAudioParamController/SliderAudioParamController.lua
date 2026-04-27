local variants = mu.variants(
  "SliderAudioParamController",
  mu.var_expr()
)

local placements = mu.vary {
  mode = {"music", "ambience"}
}

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    desc = v"Sets an audio param (music or ambience) based on {a float} value."
  }
  self:_flag_or_expr {v.noun, imperative = "set the param"}

  self.isAmbience(false)
    :desc "If checked, set an ambience param; otherwise, set a music param."
  self.param("")
    :desc "Name of the param to set."
  self.value "0.0"
    :nonempty()
    :desc(v"{Float} to set the param to.")

  for _, p in ipairs(placements) do
    p(v)
    self:_placement {
      p.mode,
      name = p"{Mode} Param Controller ({Float})",
      data = {isAmbience = p.mode == "ambience"},
    }
  end

  result[i] = self()
end
return result

