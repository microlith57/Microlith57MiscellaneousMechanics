local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local Box = {}

Box.name = "Microlith57_IntContest24/Box"
Box.depth = 100
Box.texture = "characters/theoCrystal/idle00"
Box.placements = {
    {
        name = "box",
        data = {
            speedX = 0,
            speedY = 0,
            tutorial = false
        }
    }
}

return Box