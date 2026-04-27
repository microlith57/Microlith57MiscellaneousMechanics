local variants = mu.variants(
  "SetFacingTrigger",
  mu.var_expr()
)

local abbreviations = {
  direction = {Left = "L", Right = "R"},
  invert = {[false] = "", [true] = "i"},
  continuous = {[false] = "", [true] = "c"},
  expression = {[false] = "", [true] = "e"},
}

local directions = mu.vary {dir = {"left", "right"}}

local function abbr(str, options)
  return options[str] or "?"
end

local result = {}
for i, v in ipairs(variants) do
  local self = mu.trigger {
    v.name,
    name = v"Set Player Facing ({Noun})",
    desc = "Causes the player to face either left or right."
  }

  self.direction
    :list {"Left", "Right"}
    :desc "Direction to make the player face."

  self:_flag_or_expr {v.bool, imperative = "make the player face that direction"}

  self.invertIfUnset(false)
    :name(v"Invert if {unset}")
    :desc(v"If the {bool} is {unset}, make the player face the other way.")

  self.continuous(true)
    :desc [[
      Continue to set the player's facing direction the whole time they're in the trigger, instead of once on entry.
    ]]

  local function triggerText(_, trigger)
    return (
      "Set Facing ("
      .. abbr(trigger.direction, abbreviations.direction)
      .. abbr(trigger.invertIfUnset, abbreviations.invert)
      .. abbr(trigger.continuous, abbreviations.continuous)
      .. abbr(v.bool == "expression", abbreviations.expression)
      .. ")"
    )
  end

  for _, d in ipairs(directions) do
    d(v)
    self:_placement {
      d.dir,
      name = d"Set Player Facing {(Dir, Expr?)}",
      data = {direction = d.Dir}
    }
  end

  result[i] = self {
    triggerText = triggerText
  }
end
return result
