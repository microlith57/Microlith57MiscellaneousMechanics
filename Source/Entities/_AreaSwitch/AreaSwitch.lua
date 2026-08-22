local container_names = {
  "circle",
  "diamond",
  "box",
  "dashed_circle",
  "dashed_diamond",
  "dashed_box",
  "cross",
}
local containers = {"objects/touchswitch/container"}
local textures_by_name = {}
for _, name in ipairs(container_names) do
  local tex = mu.texture {"container_" .. name}
	table.insert(containers, tex)
  textures_by_name[name] = tex
end

local colors = mu.vary {
  name = {"inactive", "active", "finish"},
  col = {"5FCDE4", "FFFFFF", "F141DF"},
}

local sprite = nil
if not mu.preprocess then
  local drawableSprite = require("structs.drawable_sprite")

  function sprite(_, entity)
    local containerResource = entity.container ~= "" and entity.container or "objects/touchswitch/conatiner"
    local containerSprite = drawableSprite.fromTexture(containerResource, entity)

    local iconResource = (entity.icon ~= "" and entity.icon or "objects/touchswitch/icon") .. "00"
    local iconSprite = drawableSprite.fromTexture(iconResource, entity)

    return {containerSprite, iconSprite}
  end
end

local self = mu.entity {
  "AreaSwitch",
  name = "Area Switch",
  depth = 2000,
  tags = false,
}

self.label "area_switch"
  :nonempty()
  :desc([[
    The session flag this area switch sets. Give the same to multiple touch switches and switch gates to group them.

    Works the same as the "flag" of flag touch switches from Maddie's Helping Hand (because that's actually what it is under the hood).
  ]])

self.persistent(false)
  :desc([[
    If enabled, the touch switch set a flag once all the switches in its group become active, meaning it will stay active if you leave and reenter the room.
  ]])

self.acceptEntities "Any"
  :list {"Any", "Player", "Box"}
  :desc("The kinds of entity that this switch reacts to.")

self.acceptStates "Any"
  :list {"Any", "Physical", "Recording"}
  :desc([[
    The "states of matter" that this switch reacts to.

    \b
    Any: Anything.
    Physical: Only original physical entities, not recordings.
    Recording: Only recordings, not physical entities.
  ]])

self.destroyBoxes(false)
  :desc("If set, this must accept only Physical Boxes. When the area switch group is completed, it will destroy the box that's activating it.")

for _, l in ipairs{"", "line"} do
  for _, c in ipairs(colors) do
    c.line = l
    self[c"{name}{Line}Color"]
      :default(c.col):color()
      :name(c"{Name}{ Line} Colour")
      :undesc()
  end
end

self.animationLength(6)
  :int():range(0, nil)
  :desc("The length of the icon spin animation, in frames. Must be at least 1.")

self.container(textures_by_name.circle)
  :info {options = containers, editable = true}
  :desc([[
    The texture of the container that holds the icon, relative to Graphics/Atlases/Gameplay.

    By convention, use a circle when the switch accepts anything; a diamond when it accepts the player; and a square when it accepts a box.
    Similarly, use a dashed line to indicate that the switch only accepts recordings.
    (Of course you are free to disregard this, it's just an aesthetic guideline.)
  ]])
-- TODO :texture()?

self.icon("objects/touchswitch/icon")
  :desc([[
    The texture of the spinning icon, relative to Graphics/Atlases/Gameplay.
    All frames (by default 6, but this can be configured with "animation length") are used when spinning; but only frame 0 is used when finished.
  ]])

self.radius(32)
  :desc([[
    The radius of the area in which the switch can be activated.
    This is where the tick lines are drawn.
  ]])
self.awareness(32)
  :desc([[
    The additional distance (added to "radius") in which the switch can 'sense' entities that might activate it.
    This has no gameplay effect, but it's a useful way to help players intuit what kinds of entity the switch wants.
  ]])
self.spacing(3.6):range(1, nil)
  :desc("The spacing between ticks when they are positioned around the circle, in pixels.")

local entities = mu.vary {
  entity = {"", "player", "box"},
  area = {"area", "player", "box"},
  tex = {"circle", "diamond", "box"}
}
local states = mu.vary {
  state = {"", "recording"},
  recording_ = {"", "_recording"},
  dashed_ = {"", "dashed_"}
}

for _, v in ipairs(entities) do
  for _, s in ipairs(states) do
    v(s)
    self:_placement {
      v"{area}_{recording_}switch",
      data = {
        acceptEntities = (v.entity ~= "") and v.entity or nil,
        acceptStates = (v.state ~= "") and v.state or nil,
        container = textures_by_name[v"{dashed_}{tex}"]
      },
    }
  end
end
self:_placement {
  "box_destroyer", name = "Area Switch (Box Destroyer)",
  data = {
    acceptEntities = "Box",
    acceptStates = "Physical",
    destroyBoxes = true,
    container = textures_by_name.cross,
    inactiveColor = "E45F5F",
    inactiveLineColor = "E45F5F",
  },
}

return self {
  sprite = sprite,
}
