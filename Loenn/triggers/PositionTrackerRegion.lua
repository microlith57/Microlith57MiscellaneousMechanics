local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  target = {
    options = {
      "Player",
      "Actor",
      "NonPlayerActor",
      "Solid"
    },
    editable = false
  },
  detection = {
    options = {
      "Within",
      "Intersecting",
      "Nearest"
    },
    editable = false
  },
  stickiness = {
    options = {
      "Free",
      "Transient",
      "UntilNewMatch",
      "UntilDeath",
      "Lifelink",
      "Soulbond"
    },
    editable = false
  },
  tracking = {
    options = {
      "Position",
      "Center",
      "TopCenter",
      "BottomCenter",
      "CenterLeft",
      "CenterRight",
      "Size"
    },
    editable = false
  },
  sliderPrefix = {validator = nonEmptyValidator}
}

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

local fieldOrder = {
  "x",
  "y",
  "width",
  "height",

  "target",
  "detection",
  "stickiness",
  "tracking",

  "sliderPrefix",
  "targettingFlag",

  "retargetIfFlag",
  "invertRetargetIfFlag",
  "retargetIfExpression",
}

local function abbr(str, options)
  return options[str] or "?"
end

local function triggerTextFlag(room, trigger)
  return (
    "Position Tracker Region (Flag) - "
    .. trigger.sliderPrefix
    .. " ("
    .. abbr(trigger.target, abbreviations.target)
    .. abbr(trigger.detection, abbreviations.detection)
    .. abbr(trigger.stickiness, abbreviations.stickiness)
    .. abbr(trigger.tracking, abbreviations.tracking)
    .. ")"
  )
end

local function triggerTextExpr(room, trigger)
  return (
    "Position Tracker Region (Expression) - "
    .. trigger.sliderPrefix
    .. " ("
    .. abbr(trigger.target, abbreviations.target)
    .. abbr(trigger.detection, abbreviations.detection)
    .. abbr(trigger.stickiness, abbreviations.stickiness)
    .. abbr(trigger.tracking, abbreviations.tracking)
    .. ")"
  )
end

return {
  {
    name = "Microlith57Misc/PositionTrackerRegion",
    placements = {
      {
        name = "positionTrackerRegion",
        data = {
          width = 16,
          height = 16,
          retargetIfFlag = "",
          invertRetargetIfFlag = false,

          target = "Actor",
          detection = "Within",
          stickiness = "Soulbond",
          tracking = "Position",

          sliderPrefix = "trackedPosition",
          targettingFlag = "",
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder,
    triggerText = triggerTextFlag,
  },
  {
    name = "Microlith57Misc/PositionTrackerRegion_Expression",
    associatedMods = {"Microlith57Misc", "FrostHelper"},
    placements = {
      {
        name = "positionTrackerRegion",
        data = {
          width = 16,
          height = 16,
          retargetIfExpression = "",

          target = "Actor",
          detection = "Within",
          stickiness = "Soulbond",
          tracking = "Position",

          sliderPrefix = "trackedPosition",
          targettingFlag = "",
        }
      }
    },
    fieldInformation = fieldInformation,
    fieldOrder = fieldOrder,
    triggerText = triggerTextExpr,
  }
}
