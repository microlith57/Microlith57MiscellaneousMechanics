local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local Box = {}

Box.name = "Microlith57Misc/Box"
Box.depth = 100
Box.texture = "objects/microlith57/misc/box/base"
Box.offset = {0, 10}
Box.placements = {
    {
        name = "box",
        data = {
            _gravityHelper = true,
            speedX = 0,
            speedY = 0,
            removeIfFlag = "",
            gravityLocked = false,
            tutorial = false
        }
    }
}
Box.ignoredFields = {
    "_gravityHelper"
}

function Box.rectangle(room, entity)
    return utils.rectangle(entity.x-10, entity.y-20, 20, 20)
end

return Box