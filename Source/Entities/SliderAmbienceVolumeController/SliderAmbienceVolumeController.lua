if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

---

local name = "SliderAmbienceVolumeController"
local placement = "sliderAmbienceVolumeController"
local variants = mu.variants(
  name,
  {{"", "Expression"}, --[[typ]] {"Slider", "Expression"}, --[[noun]] {"flag", "expression"}, --[[adj]] {"set", "truthy"}}
)

if mu.preprocess then
  for _, v in ipairs(variants) do
    local typ  = v[1][2]
    local noun = v[1][3]
    local adj  = v[1][4]

    local ent = mu.preprocess.lang.entities[v.name]
    ent.placements.name[placement] = ("Ambience Volume Controller (%s)"):format(typ)
    ent.attributes.description = {
      [noun] = ("If present, only set the volume if the %s is %s."):format(noun, adj),
      volume = ("%s to set the volume to."):format(typ)
    }
  end
  return
end

---

local result = {}
for i, v in ipairs(variants) do
  local typ = v[1][2]

  local b = mu.builder():xy()

  b.volume = "1.0"
  b.nonempty("volume")

  if typ == "Slider" then
    b.flag = ""
    b.invertFlag = false
  else
    b.expression = ""
  end

  result[i] = {
    name = v.name,
    depth = -1000000,
    texture = "objects/microlith57/misc/slider_ambience_volume_controller",
    placements = {
      b.placement(placement)
    },
    fieldOrder = b.fieldOrder,
    fieldInformation = b.fieldInformation,
  }
end
return result
