local utils = require("utils")

local RainbowLight = {}

RainbowLight.name = "Microlith57Misc/RainbowLight"
RainbowLight.depth = -1000000
RainbowLight.texture = "objects/microlith57/misc/rainbow_light"
RainbowLight.offset = {0, -1}
RainbowLight.color = {1, 1, 1, 0.5}
RainbowLight.placements = {
    {
        name = "rainbowLight",
        data = {
            alpha = 1,
            startFade = 32,
            endFade = 64
        }
    }
}
RainbowLight.fieldInformation = {
    startFade = {
        fieldType = "integer"
    },
    endFade = {
        fieldType = "integer"
    }
}

function RainbowLight.rectangle(room, entity)
    return utils.rectangle(entity.x-4, entity.y-4, 8, 8)
end

return RainbowLight
