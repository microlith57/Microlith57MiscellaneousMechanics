local variants = mu.variants(
  "PositionTrackerRegion",
  mu.var_expr()
)

local typ = mu.typology()
  :target {
    Player = "P",
    Actor = "A",
    NonPlayerActor = "N",
    Holdable = "H",
    Solid = "S"
  }
  :detection {
    Within = "W",
    Intersecting = "I",
    Nearest = "N"
  }
  :stickiness {
    Free = "F",
    Transient = "T",
    UntilNewMatch = "N",
    UntilDeath = "D",
    Lifelink = "L",
    Soulbond = "S"
  }
  :tracking {
    Position = "P",
    Center = "C",
    TopCenter = "T",
    BottomCenter = "B",
    CenterLeft = "L",
    CenterRight = "R",
    Size = "S",
    Speed = "V",
  }
  :coordinateSpace {
    World = "",
    Room = "r",
  }
  :_build()

local result = {}
for i, v in ipairs(variants) do
  local name = v"Position Tracker Region{ (Expr?)}"
  local self = mu.trigger {
    v.name,
    name = name,
    desc = "Keep track of an entity, and put its position or size into sliders."
  }

  self:_flag_or_expr {
    v.bool,
    action = "allow changing targets",
    name = v"retargetIf{Bool}",
    invert = v"invertRetargetIf{Bool}"
  }

  self.target "Actor"
    :enum {"Player", "Actor", "NonPlayerActor", "Holdables", "Solid"}
    :desc([[
      What type of entity to track.

      \b
      Player: Just the player.
      Actor: Players, holdables, and similar entities.
      NonPlayerActor: Holdables and similar entities. Uses the fact that they can move.
      Holdable: Just holdables. Uses the fact that they can be held.
      Solid: Solid entities of any kind.
    ]])

  self.detection "Within"
    :enum {"Within", "Intersecting", "Nearest"}
    :desc([[
      Whether an entity must be entirely within the region; just intersecting the region; or anywhere.
    ]])

  self.stickiness "Soulbond"
    :enum {"Free", "Transient", "UntilNewMatch", "UntilDeath", "Lifelink", "Soulbond"}
    :desc([[
      What to do once an entity is targetted.

      \b
      Free: Can retarget at any time; stop tracking when entity is no longer detected.
      Transient: Stop tracking when entity is no longer detected, and then perhaps retarget.
      UntilNewMatch: Keep tracking even after entity is no longer detected, but after this retarget if possible.
      UntilDeath: Only retarget once entity is removed.
      Lifelink: Keep entity targeted until it is removed, at which point also remove the region.
      Soulbond: Ensure entity exists when region is created, and remove region when entity is removed.
    ]])

  self.tracking "Tracking"
    :enum {"Position", "Center", "TopCenter", "BottomCenter", "CenterLeft", "CenterRight", "Size", "Speed"}
    :desc([[
      What to track. Most of these are positions on the entity's hitbox.

      \b
      Size: The output is (width, height).
      Speed: Only works on players and holdables. This is the amount the entity wants to move (px/s), not the amount it actually moves.
    ]])

  self.coordinateSpace "Relative to"
    :enum {"World", "Room"}
    :desc([[
      What the position should be measured within. Only applies when actually tracking a position.
    ]])

  self.sliderPrefix "trackedPosition"
    :nonempty()
    :desc("Set sliders whose names start with this and end in X and Y.")

  self.targettingFlag ""
    :desc("If present, set flag with this name when a target is found.")

  local function triggerText(_, trigger)
    return table.concat {name, " - ", trigger.sliderPrefix, " (", typ(trigger), ")"}
  end

  result[i] = self {
    triggerText = triggerText
  }
end
return result
