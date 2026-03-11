package.path = "selene/selene/lib/?.lua;selene/selene/lib/?/init.lua;../Source/Loenn/libraries/?.lua;../Source/?.lua" .. package.path

local selene = require("selene")
selene.load(nil, true)

mu = {
  preprocess = {locale = "en_gb"}
}

require("mu")

mu.preprocess.lang = mu.lang()
mu.preprocess.lang.mods.Microlith57MiscellaneousMechanics.name = "Microlith57's Misc. Mechanics"

local function preprocess(path)
  path = path:gsub(".lua$", "")
  require(path)
end

while true do
  local line = io.read()
  if line == nil then break end

  preprocess(line)
end

mu.print_lang(mu.preprocess.lang)
