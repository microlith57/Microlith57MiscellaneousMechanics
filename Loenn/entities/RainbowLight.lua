local utils = require("utils")

local fieldInformation = {
  startFade = {
    fieldType = "integer"
  },
  endFade = {
    fieldType = "integer"
  }
}

return {
  name = "Microlith57Misc/RainbowLight",
  depth = -1000000,
  texture = "objects/microlith57/misc/rainbow_light",
  offset = {0, -1},
  color = {1, 1, 1, 0.5},
  placements = {
    {
      name = "rainbowLight",
      data = {
        alpha = 1,
        startFade = 32,
        endFade = 64
      }
    }
  },
  fieldInformation = fieldInformation,
  rectangle = function(room, entity)
    return utils.rectangle(entity.x - 4, entity.y - 4, 8, 8)
  end
}
