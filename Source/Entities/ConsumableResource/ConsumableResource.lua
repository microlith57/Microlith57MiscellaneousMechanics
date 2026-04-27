local variants = mu.variants(
  "ConsumableResource",
  {
    {"Custom", "Stamina", "MaxStamina"},
    res     = {"Custom", "Stamina", "Max Stamina"},
    thing   = {"the resource", "stamina", "max stamina"},
    speed   = {"units/sec", "", "max stamina/sec"},
    default = {"customResource", "stamina", "maxStamina"}
  },
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.controller {
    v.name,
    name = v"Consumable Resource {(res; Expr?)}",
  }
  -- todo desc
  self:_assoc {expr = v.typ == "Expression"}

  self.resource(v.defaultRes)
    :nonempty()
    :desc "Name of this Consumable Resource entity; and also of the slider it uses."

  self.flagPrefix ""
    :desc 'If present, used as a prefix to generate "Any" / "Full" / "Low" / "Flash" flags.'

  if v.res == "Custom" then
    self.lowThreshold(20)
      :desc "Amount of the resource that's considered low."
    self.maximum(110)
      :desc "Maximum possible amount of the resource."
  elseif v.res == "MaxStamina" then
    self.lowThreshold(20)
      :desc [[
        Amount of max stamina that's considered low.
        Distinct from the low threshold for stamina itself, which is always 20.
      ]]
  end

  self["instantRefill" .. v.Bool]("")
    :desc(v[[
      When {set}, instantly refill {thing} to its maximum.

      May have unintended effects if {set} for more than a frame.
    ]])
  self["instantDrain" .. v.Bool]("")
    :desc(v[[
      When {set}, instantly set {thing} to 0.

      May have unintended effects if {set} for more than a frame.
    ]])
  if v.typ == "Flag" then
    self.invertInstantRefillFlag = false
    self.invertInstantDrainFlag = false
  end

  if v.res ~= "Stamina" then
    self.restoreCooldown(0.1)
      :desc(v"Seconds of delay before {thing} can start to refill after being drained.")

    self.restoreSpeed(60)
      :desc(v"Speed ({speed}) at which {thing} refills, in [0,∞); or -1 for instant.")
  end

  -- todo enforce nonnegative
  if v.res == "Custom" then
    self.flashRate(0.05)
      :desc [[
        Period in seconds for the "Flash" flag to flash at, in [0, ∞).

        By default this is 0.05sec, which is the same as the player at low stamina.
      ]]
  end
  if v.res ~= "Stamina" then
    self.useRawDeltaTime(false)
      :desc "If true, use real time (unaffected by slowed/sped up time); otherwise use normal game time."
    self.dieWhenConsumed(false)
      :desc(v"If true, kill the player if {thing} reaches 0.")
  end

  result[i] = self()
end
return result
