local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local Box = {}

Box.name = "Microlith57_IntContest24/Box"
Box.depth = 100
Box.texture = "objects/INTcontest24/microlith57/box"
Box.offset = {-11, -21}
Box.placements = {
    {
        name = "box",
        data = {
            speedX = 0,
            speedY = 0,
            tutorial = false,
            removeIfFlag = ""
        }
    }
}

function Box.rectangle(room, entity)
    return utils.rectangle(entity.x-10, entity.y-20, 20, 20)
end

return Box