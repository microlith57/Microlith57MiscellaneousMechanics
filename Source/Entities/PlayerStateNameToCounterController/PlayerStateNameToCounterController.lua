local self = mu.controller {
  "PlayerStateNameToCounterController",
  name = "Player State Name to Counter Controller",
  desc = "Gets the index for a player state, and also detects when the player is in that state."
}

self.stateName "StNormal"
  :nonempty()
  :desc "Name of the state to check for. Can be a modded state, and you don't have to include the 'St' prefix."

self.counter "stNormal"
  :nonempty()
  :desc "Counter to store the state index. If the state can't be found, the counter will be set to -1."

self.inStateFlag "stNormal"
  :desc "Flag to set when the player is alive and in the chosen state."
self.invertFlag = false

return self()
