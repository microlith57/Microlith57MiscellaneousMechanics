local variants = mu.variants(
  "SetFacingTrigger",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    Noun = {"Flag", "Expression"},
    adj = {"set", "truthy"},
    nadj = {"unset", "falsy"},
    par = {"", ", Expression"},
    p = {"", ", Expr"},
  }
)

local abbreviations = {
  direction = {
    Left = "L",
    Right = "R",
  },
  invert = {
    [false] = "",
    [true] = "i",
  },
  continuous = {
    [false] = "",
    [true] = "c",
  }
}

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

  self.direction(nil)
    :info {
      options = {
        "Left",
        "Right"
      },
      editable = false
    }
    :desc "Direction to make the player face."

  self:_flag_or_expr {v.noun, imperative = "make the player face that direction"}

  self.invertIfUnset(false)
    :name(v"Invert if {nadj}")
    :desc(v"If the {noun} is {nadj}, make the player face the other way.")

  self.continuous(true)
    :desc [[
      Continue to set the player's facing direction the whole time they're in the trigger, instead of once on entry.
    ]]

  local function placementName(dir)
    return "Set Player Facing (" .. dir .. v.par .. ")"
  end

  local function triggerText(room, trigger)
    return (
      "Set Facing ("
      .. abbr(trigger.direction, abbreviations.direction)
      .. abbr(trigger.invertIfUnset, abbreviations.invert)
      .. abbr(trigger.continuous, abbreviations.continuous)
      .. v.p
      .. ")"
    )
  end

  result[i] = self {
    {
      "left",
      name = "Set Player Facing (Left{par})",
      data = { direction = "Left" }
    },
    {
      "right",
      name = "Set Player Facing (Right{par})",
      data = { direction = "Right" }
    },
    triggerText = triggerText
  }
end
return result
