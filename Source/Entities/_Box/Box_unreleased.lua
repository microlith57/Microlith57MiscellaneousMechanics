local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

return {
  name = "Microlith57Misc/Box",
  depth = 100,
  texture = "objects/microlith57/misc/box/base",
  offset = {0, 10},
  placements = {
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
  },
  ignoredFields = {"_gravityHelper"},
  rectangle = function(room, entity)
    return utils.rectangle(entity.x - 10, entity.y - 20, 20, 20)
  end
}
