--[[
  preprocessing stuff.

  most of this only makes sense when run through the preprocess entrypoint, but
  sometimes there is still a return value when run from loenn.

  note that there are other preprocessing-related features in other sections.
]]

---@param f fun(): nil Function to be called once this feature is finished preprocessing.
function mu.defer(f)
  if mu.preprocess then table.insert(mu.preprocess.feature.defer, f) end
end

---@param rel string
local function abs_path_for(rel)
  return mu.preprocess.self.feat_path .. "/" .. rel:gsub("^/", '')
end

---@class _PlanMove
---@field [1] string
---@field [2] string|false?
---@field header string?
---@field footer string?
---@field absolute boolean?
---@field allow_collision boolean?

---@param tbl _PlanMove
---@return string?
function mu.plan_move(tbl)
  if not mu.preprocess then return end

  local src = tbl.absolute and tbl[1] or abs_path_for(tbl[1])

  ---@class PlannedMove
  ---@field to string?
  ---@field header string?
  ---@field footer string?
  ---@field feature string
  local move = mu.preprocess.planned_moves[src] or {}

  local dst = tbl[2]
  if dst == false then
    move.to = nil
    dst = nil
  elseif dst ~= nil then
    move.to = dst
  end

  move.header = tbl.header
  move.footer = tbl.footer

  local feature = mu.preprocess.self.feature
  if not tbl.allow_collision and move.feature and move.feature ~= feature then
    error("feature " .. feature .. " tried to overwrite file " .. dst .. " which is already set by " .. move.feature)
  end
  move.feature = feature

  mu.preprocess.planned_moves[src] = move

  return dst
end

---@overload fun(a: string): string?
---@overload fun(a: {[1]: string, [2]: string?, header: string?, footer: string?}): string
function mu.plan_move_self(a)
  if not mu.preprocess then return end

  local tbl = type(a) == "table" and a or {a}
  tbl[2] = tbl[1]
  tbl[1] = mu.preprocess.self.rel_path
  return mu.plan_move(tbl)
end

---@param tbl {[1]: string, [2]: string, atlas: string?, only_editor: boolean?}
function mu.texture(tbl)
  local src = tbl[1]
  local dst = tbl[2]
  if not dst then dst = "objects/" .. mu.modpathsegment .. "/" .. src end

  if tbl.only_editor then
    dst = dst:gsub("/[^/]+$", function(part) return part:gsub("/", "/@", 1) end)
  end

  local atlas = tbl.atlas or "Gameplay"
  local abs_dst = "Graphics/Atlases/" .. atlas .. "/" .. dst .. ".png"

  mu.plan_move {src .. ".png", abs_dst}

  return dst
end
