local self = mu.entity {
  "RecorderTerminal",
  name = "Recorder Terminal",
  depth = 2000,
}

self:_texture {"terminal", folder = "recorder_terminal"}
for _, tex in ipairs {
  "terminalcolor00", "terminalcolor01",
  "scanlines",
  "screen00", "screen01", "screen02", "screen03", "screen04",
} do
  mu.texture {tex, folder = "recorder_terminal"}
end

self.color "AC3232"
  :color()

self.maxDuration(60)
  :range(1/60, nil)

return self {
  justification = {0.5, 1.0},
}
