local GravitySwitch = {}

GravitySwitch.name = "Microlith57Misc/GravitySwitch"
GravitySwitch.depth = 2000
GravitySwitch.placements = {
  {
    name = "normal",
    data = {
      gravityType = "Normal"
    }
  },
  {
    name = "inverted",
    data = {
      gravityType = "Inverted"
    }
  },
  {
    name = "toggle",
    data = {
      gravityType = "Toggle"
    }
  }
}

function GravitySwitch.texture(room, entity)
  local grav = entity.gravityType or "Toggle"

  if grav == "Normal" then
    return "objects/GravityHelper/gravitySwitch/switch12"
  elseif grav == "Inverted" then
    return "objects/GravityHelper/gravitySwitch/switch01"
  else
    return "objects/GravityHelper/gravitySwitch/toggle01"
  end
end

return GravitySwitch