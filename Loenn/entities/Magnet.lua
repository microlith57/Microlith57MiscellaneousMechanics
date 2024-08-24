local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local Magnet = {}

Magnet.name = "Microlith57Misc/Magnet"
-- Magnet.depth = 100
-- Magnet.texture = "objects/microlith57/misc/box/normal00"
-- Magnet.offset = {0, 10}
Magnet.placements = {
    {
        name = "monopole_plus",
        data = {
            width = 16,
            height = 16,
            facing = "MonopolePlus",
            radius = 64,
            strength = 1000,
            particles = 32
        }
    },
    {
        name = "monopole_minus",
        data = {
            width = 16,
            height = 16,
            facing = "MonopoleMinus",
            radius = 64,
            strength = 1000,
            particles = 32
        }
    },
    {
        name = "up",
        data = {
            width = 16,
            height = 16,
            facing = "Up",
            radius = 64,
            strength = 1000,
            particles = 32
        }
    },
    {
        name = "down",
        data = {
            width = 16,
            height = 16,
            facing = "Down",
            radius = 64,
            strength = 1000,
            particles = 32
        }
    },
    {
        name = "left",
        data = {
            width = 16,
            height = 16,
            facing = "Left",
            radius = 64,
            strength = 1000,
            particles = 32
        }
    },
    {
        name = "right",
        data = {
            width = 16,
            height = 16,
            facing = "Right",
            radius = 64,
            strength = 1000,
            particles = 32
        }
    }
}

return Magnet