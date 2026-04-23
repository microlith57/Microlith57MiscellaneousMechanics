local self = mu.entity {
  "RainbowLight",
  name = "Rainbow Light",
}
self:_texture()

self.alpha = 1.0
self.startFade(32):int()
self.endFade(64):int()

local rectangle = nil
if not mu.preprocess then
  local utils = require("utils")
  function rectangle(room, entity)
    return utils.rectangle(entity.x - 4, entity.y - 4, 8, 8)
  end
end

return self {
  offset = {0, -1},
  color = {1, 1, 1, 0.5},
  rectangle = rectangle,
}
