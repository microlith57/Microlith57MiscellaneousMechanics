if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local self = mu.entity {
  "HoldablePriorityController",
  name = "Holdable Priority Controller",
  desc = [[
    When the player could pick up multiple holdables, this prefers the closest
    one to the player's hand position.
  ]],
  depth = false, tags = false
}
local offset_desc = [[
  Position to most prefer holdables at, relative to the player's center, in the
  current facing direction.

  Think of it as being where the hand would be when picking something up.
]]

self.checkOffsetX = 6
self.checkOffsetY = 0
self.checkOffsetX.desc = offset_desc
self.checkOffsetY.desc = offset_desc

return {
  name = self.name,
  depth = -1000000,
  texture = "objects/microlith57/misc/holdable_priority_controller",
  placements = {self()}
}
