local utils = require("utils")

return {
  name = "Microlith57Misc/PressureSensor",
  depth = -13000,
  placements = {
    {
      name = "pressureSensor",
      data = {
        width = 16,
        height = 16,
        buttons = "Top",
        label = "pressure_sensor",
        inactiveColor = "5FCDE4",
        activeColor = "F141DF"
      }
    }
  }
}
