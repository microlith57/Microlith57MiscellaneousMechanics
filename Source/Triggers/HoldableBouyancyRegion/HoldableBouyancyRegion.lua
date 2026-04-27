local variants = mu.variants(
  "HoldableBouyancyRegion",
  mu.var_expr()
)

-- todo spelling mistake

local result = {}
for i, v in ipairs(variants) do
  local self = mu.trigger {
    v.name,
    name = v"Holdable Bouyancy Region ({Float})",
    desc = "Causes holdables (and maybe the player) to experience a bouyancy force."
  }
  self:_flag_or_expr {v.noun, imperative = "apply bouyancy"}

  self.minForce "0.0"
    :nonempty()
    :desc(v"{Float} for how much force to exert (px/sec^2) when the holdable is just touching the surface.")

  self.maxForce "600.0"
    :nonempty()
    :desc(v"{Float} for how much force to exert (px/sec^2) when the holdable is fully submerged.")

  self.damping "0.5"
    :nonempty()
    :desc(v"{Float} for the coefficient controlling how much motion is slowed in the region; acts like viscosity.")

  self.alsoAffectPlayer(false)
    :desc "If set, the player will float as well as holdables."

  result[i] = self {
    triggerText = v"Holdable Bouyancy Region{par}"
  }
end
return result
