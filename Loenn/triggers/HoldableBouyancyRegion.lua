local utils = require("utils")

local nonEmptyValidator = function(s)
  return s ~= ""
end

local fieldInformation = {
  slider = {validator = nonEmptyValidator},
  minForce = {validator = nonEmptyValidator},
  maxForce = {validator = nonEmptyValidator},
  damping = {validator = nonEmptyValidator}
}

local fieldOrder = {
  "x", "y", "width", "height",
  "flag", "invertFlag", "expression",
  "minForce", "maxForce", "damping",
  "alsoAffectPlayer"
}

return {
  {
    name = "Microlith57Misc/HoldableBouyancyRegion",
    placements = {
      {
        name = "holdableBouyancyRegion",
        data = {
          width = 16,
          height = 16,
          flag = "",
          invertFlag = false,
          minForce = "0.0",
          maxForce = "600.0",
          damping = "0.5",
          alsoAffectPlayer = false
        }
      }
    }
  },
  {
    name = "Microlith57Misc/HoldableBouyancyRegion_Expression",
    associatedMods = {"Microlith57MiscellaneousMechanics", "FrostHelper"},
    placements = {
      {
        name = "holdableBouyancyRegion",
        data = {
          width = 16,
          height = 16,
          expression = "",
          minForce = "0.0",
          maxForce = "600.0",
          damping = "0.5",
          alsoAffectPlayer = false
        }
      }
    },
    triggerText = "Holdable Bouyancy Region (Expression)"
  }
}
