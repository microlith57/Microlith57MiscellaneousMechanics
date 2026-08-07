---@class mu
---@field preprocess mu.preprocess?
---
---@field configuration "debug"|"release"
---@field ["debug"|"release"] true?
local mu = (mu and mu.preprocess and mu) or {}
if configuration then mu.configuration = configuration end

local serialize = require("utils.serialize").serialize

if not mu.preprocess then
  local mods = require("mods")
  ---@diagnostic disable-next-line: duplicate-set-field
  function mu.library(lib)
    mods.requireFromPlugin("libraries.mu_" .. lib)(mu)
  end
end

--[[
  miscellaneous utilities.

  this is part of the preprocessing system; see the 'preprocess' directory (in
  the mod's source, not a .zip) for that entrypoint.
  the key idea is that each plugin file can be evaluated either at runtime like
  normal, or at preprocess time (before building the zip); the latter being used
  to automatically generate the lang file (and potentially extract other
  metadata).
  this is especially useful since this helper makes extensive use of
  parametrised entities; so being able to define lang entries in code is very
  useful.

  as with the rest of this mod, this is MIT licensed; if you would like to adapt
  this to work with your helper, please get in touch.
]]

mu.modname = "Microlith57Misc"
mu.modpathsegment = "microlith57/misc"

function mu.validate_nonempty(s) return s ~= "" end

---@param o table
function mu.pp(o)
  local _, val = serialize(o)
  print(val)
end

mu.library "assoc"
mu.library "builder"
mu.library "fmt"
mu.library "lang"
mu.library "preprocess"
mu.library "variants"

return mu
