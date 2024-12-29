local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local AreaSwitch = {}

AreaSwitch.name = "Microlith57Misc/AreaSwitch"
AreaSwitch.depth = 2000
AreaSwitch.placements = {
    {
        name = "area_switch",
        data = {
            label = "area_switch",
            persistent = "false",
            activationMode = "Anything",
            container = "objects/touchswitch/container",
            icon = "objects/touchswitch/icon",
            animationLength = 6,
            inactiveColor = "5FCDE4",
            activeColor = "FFFFFF",
            finishColor = "F141DF",
            inactiveLineColor = "5FCDE4",
            activeLineColor = "FFFFFF",
            finishLineColor = "F141DF",
            radius = 32,
            awareness = 32
        }
    },
    {
        name = "box_switch",
        data = {
            label = "area_switch",
            persistent = "false",
            activationMode = "BoxOnly",
            container = "objects/microlith57/misc/touchswitch/container_box",
            icon = "objects/touchswitch/icon",
            animationLength = 6,
            inactiveColor = "5FCDE4",
            activeColor = "FFFFFF",
            finishColor = "F141DF",
            inactiveLineColor = "5FCDE4",
            activeLineColor = "FFFFFF",
            finishLineColor = "F141DF",
            radius = 32,
            awareness = 32
        }
    },
    {
        name = "box_destroyer",
        data = {
            label = "area_switch",
            persistent = "false",
            activationMode = "DestroysBox",
            container = "objects/microlith57/misc/touchswitch/container_cross",
            icon = "objects/touchswitch/icon",
            animationLength = 6,
            inactiveColor = "5FCDE4",
            activeColor = "FFFFFF",
            finishColor = "F141DF",
            inactiveLineColor = "5FCDE4",
            activeLineColor = "FFFFFF",
            finishLineColor = "F141DF",
            radius = 32,
            awareness = 32
        }
    }
}

AreaSwitch.fieldInformation = {
    activationMode = {
        options = {
            "Anything",
            "BoxOnly",
            "DestroysBox"
        },
        editable = false
    },
    inactiveColor = {
        fieldType = "color"
    },
    activeColor = {
        fieldType = "color"
    },
    finishColor = {
        fieldType = "color"
    },
    inactiveLineColor = {
        fieldType = "color"
    },
    activeLineColor = {
        fieldType = "color"
    },
    finishLineColor = {
        fieldType = "color"
    },
    animationLength = {
        fieldType = "integer"
    }
}

function AreaSwitch.sprite(room, entity)
    local containerResource = entity.container ~= "" and entity.container or "objects/touchswitch/conatiner"
    local containerSprite = drawableSprite.fromTexture(containerResource, entity)

    local iconResource = (entity.icon ~= "" and entity.icon or "objects/touchswitch/icon") .. "00"
    local iconSprite = drawableSprite.fromTexture(iconResource, entity)

    return {containerSprite, iconSprite}
end

return AreaSwitch
