local utils = require("utils")

local PressureSensor = {}

PressureSensor.name = "Microlith57Misc/PressureSensor"
PressureSensor.depth = -13000
PressureSensor.placements = {
    {
        name = "pressureSensor",
        data = {
            width = 16,
            height = 16,
            buttons = "Top",
            label = "pressure_sensor",
            inactiveColor = "5FCDE4",
            activeColor = "F141DF",
        }
    }
}

return PressureSensor
