local utils = require("utils")

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
          minForce = 0,
          maxForce = 600,
          damping = 0.5,
          alsoAffectPlayer = false
        }
      }
    }
  },
  {
    name = "Microlith57Misc/HoldableBouyancyRegion_Expression",
    placements = {
      {
        name = "holdableBouyancyRegion",
        data = {
          width = 16,
          height = 16,
          expression = "",
          minForce = 0,
          maxForce = 600,
          damping = 0.5,
          alsoAffectPlayer = false
        }
      }
    }
  }
}
