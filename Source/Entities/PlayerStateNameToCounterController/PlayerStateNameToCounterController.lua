if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local self = mu.entity {
  "PlayerStateNameToCounterController",
  name = "Player State Name to Counter Controller",
  desc = "Gets the index for a player state, and also detects when the player is in that state."
}

self.stateName = "StNormal"
self.stateName:nonempty()
self.stateName.desc = "Name of the state to check for. Can be a modded state, and you don't have to include the 'St' prefix."

self.counter = "stNormal"
self.counter:nonempty()
self.counter.desc = "Counter to store the state index. If the state can't be found, the counter will be set to -1."

self.inStateFlag = "stNormal"
self.inStateFlag.desc = "Flag to set when the player is alive and in the chosen state."
self.invertFlag = false

return {
  {
    name = self.name,
    depth = -1000000,
    texture = "objects/microlith57/misc/player_state_name_to_counter_controller",
    placements = {self()},
    fieldOrder = self.fieldOrder,
    fieldInformation = self.fieldInformation
  }
}
