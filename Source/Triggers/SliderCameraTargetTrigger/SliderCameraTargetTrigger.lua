local variants = mu.variants(
  "SliderCameraOffsetTrigger",
  mu.var_expr()
)

local coords = mu.vary {
  axis = {"x", "y"},
  dir = {"horizontal", "vertical"}
}

local result = {}
for i, v in ipairs(variants) do
  local name = v"Camera Target {(Float)}"
  local self = mu.trigger {
    v.name,
    name = name,
    desc = v"Sets the camera target based on {float} values."
  }
  self:_flag_or_expr {v.noun, action = "set the camera target"}
  self:_position_mode {"positionMode", action = "affect the camera target"}

  -- todo
  -- self:_flag_or_expr {v.bool, name = v"delete{Bool}", invertFlag = false, desc = v"When this {bool} is {set}, the trigger will delete itself."}

  self:_interleave(coords, {"{axis}Only", "target{Axis}", "lerpStrength{Axis}"})
  for _, c in ipairs(coords) do
    c(v)

    self[c"{axis}Only"]
      :default(false)
      :desc(c"If set, only affect the camera in the {Axis} axis.")

    self[c"target{Axis}"]
      :default ""
      :desc(c[[
        {Float} {containing} the {Axis} coordinate of the camera target.
        If absent, uses the {Axis} coordinate of the node.
      ]])

    self[c"lerpStrength{Axis}"]
      :default "1.0"
      :desc(c"{Float} {containing} the {dir} lerp strength.")
  end

  self.snapMode "NeverSnap"
    :list {
      "NeverSnap",
      "SnapWhenInitiallyEnabled",
      "AlwaysSnap",
      "AlwaysSnapIgnoringRoomBounds"
    }
    :desc([[
      When to snap the camera so its position exactly matches the target (rather than trying to catch up to it).
      This takes into account lerp; so if you want to snap the camera exactly to the specified position you need lerp strength 1.

      NeverSnap: Don't snap.
      SnapWhenInitiallyEnabled: Snap when the trigger becomes enabled (where the previous frame it wasn't).
      AlwaysSnap: Snap every frame.
      AlwaysSnapIgnoringRoomBounds: Snap every frame, unconstrained by the room's bounds.
    ]])

  result[i] = self {
    category = "camera",
    nodeLimits = {0, 1},
    triggerText = name,
  }
end
return result
