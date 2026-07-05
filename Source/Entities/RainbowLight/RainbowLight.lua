local self = mu.entity {
  "RainbowLight",
  name = "Rainbow Light",
}
self:_texture()

self.alpha(1.0)
  :desc "How strong the light is overall, in [0.0, 1.0], where 0.0 means invisible and 1.0 means fully lit."
self.startFade(32):int()
  :desc "How far from the light source that the light should start to fade; closer than this its effect will be uniform."
self.endFade(64):int()
  :desc "How far should reach overall; further than this it will have no effect."

local rectangle = nil
if not mu.preprocess then
  local utils = require("utils")
  function rectangle(_, entity)
    return utils.rectangle(entity.x - 4, entity.y - 4, 8, 8)
  end
end

return self {
  offset = {0, -1},
  color = {1, 1, 1, 0.5},
  rectangle = rectangle,
}
