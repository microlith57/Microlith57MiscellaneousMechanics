local enums = require("consts.celeste_enums")

local variants = mu.variants(
  "SliderCameraOffsetTrigger",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    adj = {"set", "truthy"},
  }
)

local result = {}
for i, v in ipairs(variants) do
  local name = v"Camera Offset ({Noun})"
  local self = mu.trigger {
    v.name,
    name = name,
    desc = v"Sets the camera offset based on {noun} values."
  }
  self:_flag_or_expr {v.noun, imperative = "apply the camera offset"}

  self.offsetFromX "0.0"
    :nonempty()
    :desc "Slider for the first X position of the camera offset."
  self.offsetFromY "0.0"
    :nonempty()
    :desc "Slider for the first Y position of the camera offset."

  self.offsetToX "0.0"
    :nonempty()
    :desc "Slider for the second X position of the camera offset."
  self.offsetToY "0.0"
    :nonempty()
    :desc "Slider for the second Y position of the camera offset."

  self:_position_mode {desc = "Determines which direction the player position will affect the offset in."}

  self.coarse(true)
    :desc [[
      If checked, units are in the usual camera offset units. Otherwise they are in pixels.

      Camera offset units are 48px in the X direction and 32px in the Y direction.
    ]]

  result[i] = self {
    category = "camera",
    triggerText = name,
  }
end
return result
