local variants = mu.variants(
  "PositionTrackerRegion",
  mu.var_expr()
)

local abbreviations = {
  target = {
    Player = "P",
    Actor = "A",
    NonPlayerActor = "N",
    Solid = "S",
  },
  detection = {
    Within = "W",
    Intersecting = "I",
    Nearest = "N",
  },
  stickiness = {
    Free = "F",
    Transient = "T",
    UntilNewMatch = "N",
    UntilDeath = "D",
    Lifelink = "L",
    Soulbond = "S",
  },
  tracking = {
    Position = "P",
    Center = "C",
    TopCenter = "T",
    BottomCenter = "B",
    CenterLeft = "L",
    CenterRight = "R",
    Size = "S",
  },
}

local function abbr(str, options)
  return options[str] or "?"
end

local result = {}
for i, v in ipairs(variants) do
  local name = v"Position Tracker Region {(Expr?)}"
  local self = mu.trigger {
    v.name,
    name = name,
    desc = "Keep track of an entity, and put its position or size into sliders."
  }

  local function triggerText(_, trigger)
    return (
      name .. " - "
      .. trigger.sliderPrefix
      .. " ("
      .. abbr(trigger.target, abbreviations.target)
      .. abbr(trigger.detection, abbreviations.detection)
      .. abbr(trigger.stickiness, abbreviations.stickiness)
      .. abbr(trigger.tracking, abbreviations.tracking)
      .. ")"
    )
  end

  self:_flag_or_expr {
    v.bool,
    imperative = "allow changing targets",
    name = v"retargetIf{Bool}",
    invert = v"invertRetargetIf{Bool}"
  }

  self.target "Actor"
    :list {"Player", "Actor", "NonPlayerActor", "Solid"}
    :desc [[
      What type of entity to track.

      \b
      Player: Just the player.
      Actor: Players, holdables, and similar entities.
      NonPlayerActor: Holdables and similar entities.
      Solid: Solid entities of any kind.
    ]]

  self.detection "Within"
    :list {"Within", "Intersecting", "Nearest"}
    :desc [[
      Whether an entity must be entirely within the region; just intersecting the region; or anywhere.
    ]]

  self.stickiness "Soulbond"
    :list {"Free", "Transient", "UntilNewMatch", "UntilDeath", "Lifelink", "Soulbond"}
    :desc [[
      What to do once an entity is targetted.

      \b
      Free: Can retarget at any time; stop tracking when entity is no longer detected.
      Transient: Stop tracking when entity is no longer detected, and then perhaps retarget.
      UntilNewMatch: Keep tracking even after entity is no longer detected, but after this retarget if possible.
      UntilDeath: Only retarget once entity is removed.
      Lifelink: Keep entity targeted until it is removed, at which point also remove the region.
      Soulbond: Ensure entity exists when region is created, and remove region when entity is removed.
    ]]

  self.tracking "Position"
    :list {"Position", "Center", "TopCenter", "BottomCenter", "CenterLeft", "CenterRight", "Size"}
    :desc 'What position to track; or "Size" for width/height.'

  self.sliderPrefix "trackedPosition"
    :nonempty()
    :desc "Set sliders whose names start with this and end in X and Y."

  self.targettingFlag ""
    :desc "If present, set flag with this name when a target is found."

  result[i] = self {
    triggerText = triggerText
  }
end
return result
