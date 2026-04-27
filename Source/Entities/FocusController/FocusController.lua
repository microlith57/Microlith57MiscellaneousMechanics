local variants = mu.variants(
  "FocusController",
  {
    {"", "Button", "Expression"},
    typ  = {"Slider", "Button", "Expression"},
    bool = {"flag", "flag", "expression"},
    set  = {"set", "set", "truthy"}
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Focus Controller ({typ})"
  }
  self:_flag_or_expr {v.bool, imperative = "allow focusing", name = "enabled{Bool}", invert = "invertEnabled{Bool}"}

  if v.typ == "Flag" then
    self.activeFlag "tryingToFocus"
      :nonempty()
      :desc "If possible, focus whenever this flag is set."
    self.invertActiveFlag = false
  elseif v.typ == "Expression" then
    self.activeExpression "$input.grab"
      :nonempty()
      :desc "If possible, focus whenever this expression is truthy."
  end

  self.slider "focus"
    :nonempty()
    :desc "Name of the slider to set based on the focus amount, in [0,1], where 0 represents normal and 1 represents fully focused."

  self.flagPrefix ""
    :desc 'If present, used as a prefix to generate "Trying" / "Focusing" / "AnyFocus" / "FullFocus" flags.'

  self.consumptionResourceName ""
    :desc "If present, link to the Consumable Resource entity with this name."
  self.consumptionRate(12.0)
    :desc "How fast (units/sec) to consume the linked resource."
  self.unfocusWhenResourceLow(false)
    :desc "If true, gradually unfocus when the resource is running low."

  self.fadeDuration(1.0)
    :desc "How many seconds to take to fade between normal and focusing, in [0-∞)."
  self:_raw_delta_time()

  result[i] = self()
end
return result
