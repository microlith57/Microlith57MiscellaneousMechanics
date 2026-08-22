local self = mu.entity {
  "RecorderTerminal",
  name = "Recorder Terminal",
  depth = 2000,
}
self:_texture "terminal"

self.color "AC3232"
  :color()

self.maxDuration(60)
  :range(1/60, nil)

return self {
  justification = {0.5, 1.0},
}
