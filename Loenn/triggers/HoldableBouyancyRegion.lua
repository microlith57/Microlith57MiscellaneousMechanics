local utils = require("utils")

local HoldableBouyancyRegion = {}

HoldableBouyancyRegion.name = "Microlith57Misc/HoldableBouyancyRegion"
HoldableBouyancyRegion.placements = {
    {
        name = "holdableBouyancyRegion",
        data = {
            width = 16,
            height = 16,
            minForce = 0,
            maxForce = 600,
            damping = 0.5,
            alsoAffectPlayer = false
        }
    }
}

return HoldableBouyancyRegion
