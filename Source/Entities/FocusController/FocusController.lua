if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "FocusController",
  {
    {"", "Button", "Expression"},
    typ  = {"Slider", "Button", "Expression"},
    noun = {"flag", "flag", "expression"},
    Noun = {"Flag", "Flag", "Expression"},
    adj  = {"set", "set", "truthy"}
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Focus Controller ({typ})"
  }

  self["enabled" .. v.Noun] = ""
  self["enabled" .. v.Noun].desc = v"If present, only allow focusing when the {noun} is {adj}."
  if v.typ ~= "Expression" then self.invertEnabledFlag = false end

  if v.typ == "Flag" then
    self.activeFlag = "tryingToFocus"
    self.activeFlag:nonempty()
    self.activeFlag.desc = "If possible, focus whenever this flag is set."
    self.invertActiveFlag = false
  elseif v.typ == "Expression" then
    self.activeExpression = "$input.grab"
    self.activeExpression:nonempty()
    self.activeExpression.desc = "If possible, focus whenever this expression is truthy."
  end

  self.slider = "focus"
  self.slider:nonempty()
  self.slider.desc = "Name of the slider to set based on the focus amount, in [0,1], where 0 represents normal and 1 represents fully focused."

  self.flagPrefix = ""
  self.flagPrefix.desc = 'If present, used as a prefix to generate "Trying" / "Focusing" / "AnyFocus" / "FullFocus" flags.'

  self.consumptionResourceName = ""
  self.consumptionResourceName.desc = "If present, link to the Consumable Resource entity with this name."
  self.consumptionRate = 12.0
  self.consumptionRate.desc = "How fast (units/sec) to consume the linked resource."
  self.unfocusWhenResourceLow = false
  self.unfocusWhenResourceLow.desc = "If true, gradually unfocus when the resource is running low."

  self.fadeDuration = 1.0
  self.fadeDuration.desc = "How quickly to fade between normal and focusing, in [0-∞)."
  self.useRawDeltaTime = false
  self.useRawDeltaTime.desc = "If true, use real time (unaffected by slowed/sped up time); otherwise use normal game time."

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.typ == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/focus_controller",
    placements = {self()},
    fieldOrder = self.fieldOrder,
    fieldInformation = self.fieldInformation
  }
end
return result
