---@meta

---@param self string
---@param sep string
---@return fun(): string[]
function string:split(sep) end

---@param self string
---@param n number
---@return string
function string:drop(n) end

---@param tbl table
---@return any[]
function table.keys(tbl) end

---@generic T : table
---@param tbl T
---@return T
function table.shallowcopy(tbl) end
