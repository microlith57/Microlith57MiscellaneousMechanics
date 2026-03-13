if not mu then
  local mods = require("mods")
  local mu = mods.requireFromPlugin("libraries.utils")
end

local variants = mu.variants(
  "LockPauseController",
  {
    {"", "Expression"},
    noun = {"flag", "expression"},
    adj = {"set", "truthy"},
    par = {"", "Expression; "}
  }
)

local result = {}
for i, v in ipairs(variants) do
  local self = mu.entity {v.name}

  self[v.noun] = "lockPause"
  self[v.noun].desc = v"If present, only lock if the {noun} is {adj}."
  if v.noun == "flag" then self.invertFlag = false end

  self.mode = nil
  self.mode.info = {
    options = {
      "LockRetry",
      "LockSaveQuit",
      "LockRetryAndSaveQuit",
      "LockPauseMenu",
      "LockRetrySaveQuitAndPauseMenu"
    },
    editable = false
  }
  self.mode.desc = [[
    What to lock.

    LockRetry: Grey out the Retry button and prevent the retry keybind;
    LockSaveQuit: Grey out the Save and Quit button;
    LockRetryAndSaveQuit: Grey out both the Retry and Save and Quit buttons;
    LockPauseMenu: Prevent opening the pause menu entirely;
    LockRetrySaveQuitAndPauseMenu: Prevent opening the pause menu, and also prevent using keybinds to retry or save+quit.
  ]]

  self.unlockWhenControllerRemoved = true
  self.unlockWhenControllerRemoved.desc = "Whether to reenable the locked features when the controller is removed (eg. when transitioning into a different room)."

  self.inhibitGBJPrevention = false
  self.inhibitGBJPrevention.desc = [[
    By default, Everest prevents death-on-spawn softlocks by forcefully pausing under certain conditions (this is GBJ prevention).
    This option inhibits GBJ prevention, ensuring the game does not forcefully pause.
    Only relevant if you disable opening the menu entirely.

    WARNING: This disables a softlock prevention mechanism! Use with care!
  ]]

  result[i] = {
    name = self.name,
    associatedMods = mu.assoc {expr = v.Noun == "Expression"},
    depth = -1000000,
    texture = "objects/microlith57/misc/lock_pause_controller",
    placements = {
      self {
        "lockRetryController",
        name = v"Lock Pause Controller ({par}Disable Retry)",
        data = {mode = "LockRetry"}
      },
      self {
        "lockSaveQuitController",
        name = v"Lock Pause Controller ({par}Disable Save+Quit)",
        data = {mode = "LockSaveQuit"}
      },
      self {
        "lockPauseController",
        name = v"Lock Pause Controller ({par}Disable Menu)",
        data = {mode = "LockPause"}
      }
    },
    fieldOrder = self.fieldOrder,
    fieldInformation = self.fieldInformation
  }
end
return result
