local self = mu.entity {
  "Box",
  name = "Box",
  texture = "base",
  depth = 100,
}

for _, t in ipairs {"indicator", "playback"} do
  for _, l in ipairs {"", "_locked"} do
    for _, i in ipairs {"00", "01", "02"} do
      mu.texture {t .. l .. i}
    end
  end
end

self._gravityHelper(true):ignore()

self.speedX(0)
self.speedY(0)

self.removeIfFlag("")
self.gravityLocked(false)
self.tutorial(false)

local rectangle
if not mu.preprocess then
  local utils = require("utils")
  function rectangle(_, entity)
    return utils.rectangle(entity.x - 10, entity.y - 20, 20, 20)
  end
end

return self {
  offset = {0, 10},
  rectangle = rectangle,
}
