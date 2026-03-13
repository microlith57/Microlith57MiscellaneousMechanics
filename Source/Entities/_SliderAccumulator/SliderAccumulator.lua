if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local self = mu.entity {
  "SliderAccumulator",
  name = "Slider Accumulator",
  desc = "Tracks how a slider changes over time."
}

self.input = "input"
self.input:nonempty()
self.input.desc = "Slider to use as the input."

self.output = "output"
self.output:nonempty()
self.output.desc = "Slider to use as the output."

self.resetInput = nil
self.resetInput:optional()
self.resetInput.desc = "If set, reset the input to this number at the start of every frame."

self.resetOutput = nil
self.resetOutput:optional()
self.resetOutput.desc = "If set, reset the output to this number at the start of every frame."

local operations = {"Sum", "AbsSum", "Product", "AbsProduct", "Average"}

self.operation = "Sum"
self.operation.info = {
  options = operations,
  editable = false
}
self.operation.desc = [[
  How to change the output in response to a change of the input.

  Given a change 'delta' (either the value of input directly, or the change in
  that value, depending on whether 'relative' is enabled):

  Sum: Add delta to output;
  AbsSum: Add the size of delta to output (so negative deltas still increase it);
  Product: Multiply output by delta;
  AbsProduct: Multiply output by the size of delta;
  Average: Compute a running average of all delta values.
]]

self.relative = true
self.relative.desc = "If set, adjust the output based on the change in input, rather than the input directly."

return {
  name = self.name,
  depth = -1000000,
  texture = "objects/microlith57/misc/slider_accumulator",
  placements = {
    self {
      "derivative",
      name = "Slider Accumulator (Derivative)",
      data = {
        operation = "Sum",
        relative = true,
        resetOutput = 0
      }
    },
    self {
      "derivative",
      name = "Slider Accumulator (Average Frame)",
      data = {
        operation = "Average",
        relative = false,
        resetOutput = 0
      }
    },
    -- self {
    --   "integral",
    --   name = "Slider Accumulator (Integral)",
    --   data = {
    --     operation = "Sum",
    --     relative = false,
    --     lazy = true,
    --     resetInput = 0
    --   }
    -- }
  },
}
