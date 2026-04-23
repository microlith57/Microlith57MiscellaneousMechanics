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
  local self = mu.controller {
    v.name,
  }
  -- todo: desc
  self:_flag_or_expr {v.noun, imperative = "lock", default = "lockPause"}

  self.mode(nil)
    :info {
      options = {
        "LockRetry",
        "LockSaveQuit",
        "LockRetryAndSaveQuit",
        "LockPauseMenu",
        "LockRetrySaveQuitAndPauseMenu"
      },
      editable = false
    }
    :desc [[
      What to lock.

      \b
      LockRetry: Grey out the Retry button and prevent the retry keybind;
      LockSaveQuit: Grey out the Save and Quit button;
      LockRetryAndSaveQuit: Grey out both the Retry and Save and Quit buttons;
      LockPauseMenu: Prevent opening the pause menu entirely;
      LockRetrySaveQuitAndPauseMenu: Prevent opening the pause menu, and also prevent using keybinds to retry or save+quit.
    ]]

  self.unlockWhenControllerRemoved(true)
    :desc "Whether to reenable the locked features when the controller is removed (eg. when transitioning into a different room)."

  self.inhibitGBJPrevention(false)
    :desc [[
      By default, Everest prevents death-on-spawn softlocks by forcefully pausing under certain conditions (this is GBJ prevention).
      This option inhibits GBJ prevention, ensuring the game does not forcefully pause.
      Only relevant if you disable opening the menu entirely.

      WARNING: This disables a softlock prevention mechanism! Use with care!
    ]]

  result[i] = self {
    {
      "lockRetryController",
      name = v"Lock Pause Controller ({par}Disable Retry)",
      data = {mode = "LockRetry"}
    },
    {
      "lockSaveQuitController",
      name = v"Lock Pause Controller ({par}Disable Save+Quit)",
      data = {mode = "LockSaveQuit"}
    },
    {
      "lockPauseController",
      name = v"Lock Pause Controller ({par}Disable Menu)",
      data = {mode = "LockPause"}
    }
  }
end
return result
