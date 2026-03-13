if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local self = mu.entity {
  "RainbowLight",
  name = "Rainbow Light"
}

self.alpha = 1.0
self.startFade = 32
self.startFade:int()
self.endFade = 64
self.endFade:int()

local result = {
  name = self.name,
  depth = -1000000,
  texture = "objects/microlith57/misc/rainbow_light",
  offset = {0, -1},
  color = {1, 1, 1, 0.5},
  placements = {self()},
  fieldOrder = self.fieldOrder,
  fieldInformation = self.fieldInformation
}

if not mu.preprocess then
  local utils = require("utils")
  function result.rectangle(room, entity)
    return utils.rectangle(entity.x - 4, entity.y - 4, 8, 8)
  end
end

return result
