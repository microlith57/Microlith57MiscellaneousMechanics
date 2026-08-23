local variants = mu.variants(
  "SliderSoundSource",
  mu.var_expr()
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {
    v.name,
    name = v"Slider Sound Source ({Float})",
    desc = "Plays a sound, with position / volume / parameters controlled by sliders."
  }
    :_extra {depth = -1000000}
    :_texture {only_editor = true}
  -- TODO common fields

  self:_flag_or_expr {
    v.bool,
    name = v"enable{Bool}",
    invert = "invertEnable",
    desc = [[
      If present, play when the {bool} is {set} and stop when it is {unset}.

      While stopping, the sound's parameters cannot be changed.
      To stop the sound instantly, you can set its volume to 0 and then stop it on the next frame.
    ]],
  }
  -- TODO better desc

  self:_flag_or_expr {
    v.bool,
    name = v"playing{Bool}",
    invert = "invertPlaying",
    desc = "If present, play when the {bool} is {set} and pause when it is {unset}.",
  }

  self.positionX "0.0"
    :nonempty()
    :desc(v"{Float} {containing} the X position of the sound.")
  self.positionY "0.0"
    :nonempty()
    :desc(v"{Float} {containing} the Y position of the sound.")

  self.positionRelative(true)
    :desc("If enabled, coordinates will be relative to the sound source. Otherwise, they will be in map coordinates.")

  self:_spacer()

  self.sound ""
    :desc("The sound event that will be played.")

  self.params ""
    :list {
      v"param:{float}",
      options = mu.list {
        sep = ":",
        count = 2,
      }
    }
    :desc(v"List of parameter names, and the {floats} to set them to.")

  self.volume "1.0"
    :nonempty()
    :desc(v"{Float} to set the sound's volume.")

  self.globalRoomCompat(false)
    :desc([[
      Enable this if placing the sound source in a global room.

      If this is ticked, when transitioning between two rooms with the same sound source (i.e. both with this ticked, and with the same event name), the transition will be seamless.
    ]])

  result[i] = self()
end
return result
