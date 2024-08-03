local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local AreaSwitch = {}

AreaSwitch.name = "Microlith57_IntContest24/AreaSwitch"
AreaSwitch.depth = 2000
AreaSwitch.placements = {
    {
        name = "area_switch",
        data = {
            flag = "area_switch",
            icon = "objects/touchswitch/icon",
            animationLength = 6,
            inactiveColor = "5FCDE4",
            activeColor = "FFFFFF",
            finishColor = "F141DF"
        }
    }
}

AreaSwitch.fieldInformation = {
    inactiveColor = {
        fieldType = "color"
    },
    activeColor = {
        fieldType = "color"
    },
    finishColor = {
        fieldType = "color"
    },
    animationLength = {
        fieldType = "integer"
    }
}

function AreaSwitch.sprite(room, entity)
    local containerSprite = drawableSprite.fromTexture("objects/touchswitch/container", entity)

    local iconResource = (entity.icon ~= "" and entity.icon or "objects/touchswitch/icon") .. "00"
    local iconSprite = drawableSprite.fromTexture(iconResource, entity)

    return {containerSprite, iconSprite}
end

return AreaSwitch