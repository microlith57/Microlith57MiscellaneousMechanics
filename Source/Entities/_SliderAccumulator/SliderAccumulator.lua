local self = mu.entity {
  "SliderAccumulator",
  name = "Slider Accumulator",
  desc = "Tracks how a slider changes over time."
}

self.input "input"
  :nonempty()
  :desc("Slider to use as the input.")

self.output "output"
  :nonempty()
  :desc("Slider to use as the output.")

self.resetInput(nil)
  :info {fieldType = "number"}
  :optional()
  :desc("If set, reset the input to this number at the start of every frame." )

self.resetOutput(nil)
  :info {fieldType = "number"}
  :optional()
  :desc("If set, reset the output to this number at the start of every frame." )

self.operation "Sum"
  :enum {"Sum", "AbsSum", "Product", "AbsProduct", "Average"}
  :desc([[
    How to change the output in response to a change of the input.

    Given a change 'delta' (either the value of input directly, or the change in
    that value, depending on whether 'relative' is enabled):

    \b
    Sum: Add delta to output;
    AbsSum: Add the size of delta to output (so negative deltas still increase it);
    Product: Multiply output by delta;
    AbsProduct: Multiply output by the size of delta;
    Average: Compute a running average of all delta values.
  ]])

self.relative(true)
  :desc("If set, adjust the output based on the change in input, rather than the input directly.")

self:_placement {
  "derivative",
  name = "Slider Accumulator (Derivative)",
  data = {
    operation = "Sum",
    relative = false,
    resetOutput = 0,
  }
}

self:_placement {
  "average_frame",
  name = "Slider Accumulator (Average Frame)",
  data = {
    operation = "Average",
    relative = false,
    resetOutput = 0,
  }
}

self:_placement {
  "integral",
  name = "Slider Accumulator (Integral)",
  data = {
    operation = "Sum",
    relative = false,
    lazy = true,
    resetInput = 0
  }
}

return self()
