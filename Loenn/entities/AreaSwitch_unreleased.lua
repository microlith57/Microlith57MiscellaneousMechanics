local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local fieldInformation = {
  acceptEntities = {
    options = {
      "Any",
      "Player",
      "Box"
    },
    editable = false
  },
  acceptStates = {
    options = {
      "Any",
      "Physical",
      "Recording"
    },
    editable = false
  },
  container = {
    options = {
      "objects/touchswitch/container",
      "objects/microlith57/misc/touchswitch/container_circle",
      "objects/microlith57/misc/touchswitch/container_dashed_circle",
      "objects/microlith57/misc/touchswitch/container_box",
      "objects/microlith57/misc/touchswitch/container_dashed_box",
      "objects/microlith57/misc/touchswitch/container_diamond",
      "objects/microlith57/misc/touchswitch/container_dashed_diamond",
      "objects/microlith57/misc/touchswitch/container_cross"
    },
    editable = true
  },
  inactiveColor = {fieldType = "color"},
  activeColor = {fieldType = "color"},
  finishColor = {fieldType = "color"},
  inactiveLineColor = {fieldType = "color"},
  activeLineColor = {fieldType = "color"},
  finishLineColor = {fieldType = "color"},
  animationLength = {fieldType = "integer"}
}

local function sprite(room, entity)
  local containerResource = entity.container ~= "" and entity.container or "objects/touchswitch/conatiner"
  local containerSprite = drawableSprite.fromTexture(containerResource, entity)

  local iconResource = (entity.icon ~= "" and entity.icon or "objects/touchswitch/icon") .. "00"
  local iconSprite = drawableSprite.fromTexture(iconResource, entity)

  return {containerSprite, iconSprite}
end

return {
  name = "Microlith57Misc/AreaSwitch",
  depth = 2000,
  placements = {
    {
      name = "area_switch",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Any",
        acceptStates = "Any",
        destroyBoxes = false,
        container = "objects/microlith57/misc/touchswitch/container_circle",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    },
    {
      name = "player_switch",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Player",
        acceptStates = "Any",
        destroyBoxes = false,
        container = "objects/microlith57/misc/touchswitch/container_diamond",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    },
    {
      name = "box_switch",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Box",
        acceptStates = "Any",
        destroyBoxes = false,
        container = "objects/microlith57/misc/touchswitch/container_box",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    },
    {
      name = "recording_switch",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Any",
        acceptStates = "Recording",
        destroyBoxes = false,
        container = "objects/microlith57/misc/touchswitch/container_dashed_circle",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    },
    {
      name = "player_recording_switch",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Player",
        acceptStates = "Recording",
        destroyBoxes = false,
        container = "objects/microlith57/misc/touchswitch/container_dashed_diamond",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    },
    {
      name = "box_recording_switch",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Box",
        acceptStates = "Recording",
        destroyBoxes = false,
        container = "objects/microlith57/misc/touchswitch/container_dashed_box",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    },
    {
      name = "box_destroyer",
      data = {
        label = "area_switch",
        persistent = false,
        acceptEntities = "Box",
        acceptStates = "Physical",
        destroyBoxes = true,
        container = "objects/microlith57/misc/touchswitch/container_cross",
        icon = "objects/touchswitch/icon",
        animationLength = 6,
        inactiveColor = "5FCDE4",
        activeColor = "FFFFFF",
        finishColor = "F141DF",
        inactiveLineColor = "5FCDE4",
        activeLineColor = "FFFFFF",
        finishLineColor = "F141DF",
        radius = 32,
        awareness = 32
      }
    }
  },
  fieldInformation = fieldInformation,
  sprite = sprite
}
