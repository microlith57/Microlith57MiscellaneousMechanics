--[[
  language file builder.

  it's annoying to maintain three separate files per entity (csharp, lua, lang).
  this is part of a system to build language entries inside plugins' lua code,
  resulting in only two places to coordinate, and allowing entries to be written
  in a more powerful and convenient way.
  it's useful to be able to just set keys without worrying about creating the
  intermediate tables, so this metatable magic allows that. (it's inspired by
  the nixpkgs module system's attrset merging!)

  given:
  > local l = mu.lang()
  > l.path.to.some.key = "value"
  > l.merge = { [1] = 1 }
  > l.merge = { [2] = 2 }
  > l.merge[3] = 3

  yields:
  > {
  >   path = { to = { some = { key = "value" } } },
  >   merge = { 1, 2, 3 }
  > }
]]

-- todo: deal with case where same entry is set twice

local indent_ruler = 80

---@param val string
local function prepare_lang_entry(val)
  local lines = tostring(val):split("\n")() ---@type string[]
  if #lines == 0 then return "" end
  local indent = #(lines[1]:match("^%s*"))

  local res = {} ---@type string[]
  local paragraph = {} ---@type string[]
  local reindent = true

  local function push_paragraph()
    if #paragraph == 0 then return end

    if #res > 0 then table.insert(res, "") end

    if reindent then
      local words = {} ---@type string[]
      for _, line in ipairs(paragraph) do
        local words_in_line = line:split(" ")()
        for _, word in ipairs(words_in_line) do
          table.insert(words, word)
        end
      end

      local line = {} ---@type string[]
      local len = 0
      local function push_line()
        table.insert(res, table.concat(line, " "))
        line = {}
        len = 0
      end

      for _, word in ipairs(words) do
        local next_len = len + #word + 1
        if next_len <= indent_ruler then
          table.insert(line, word)
          len = next_len
        else
          push_line()

          table.insert(line, word)
          len = #word

          if len > indent_ruler then push_line() end
        end
      end
      push_line()
    else
      for _, line in ipairs(paragraph) do
        table.insert(res, line)
      end
    end
    paragraph = {}
    reindent = true
  end

  for _, line in ipairs(lines) do
    local this_indent = #(line:match("^%s*")) - indent
    if this_indent < 0 then this_indent = 0 end

    line = line:gsub("^%s*", ""):gsub("%s*$", "")
    if line == "\b" or line == "\\b" then
      reindent = false
    elseif line == "" then
      push_paragraph()
    else
      table.insert(paragraph, line)
    end
  end
  push_paragraph()

  return table.concat(res, "\\n")
end

---@class Lang
---@field [string] Lang | string
local Lang = {}

---@param key string
---@return Lang
function Lang:__index(key)
  local val = {}
  setmetatable(val, Lang)
  rawset(self, key, val)
  return val
end
---@param key string
---@param val string
function Lang:__newindex(key, val)
  if type(val) == "table" then
    local current_val = self[key]
    for k, v in pairs(val) do
      current_val[k] = v
    end
  elseif val == nil then
    rawset(self, key, nil)
  else
    local entry = prepare_lang_entry(val)
    rawset(self, key, entry)
  end
end

---@param l table
---@return Lang
function mu.lang(l)
  l = l or {}
  setmetatable(l, Lang)
  return l
end

---@param l Lang
function mu.print_lang(l, f)
  local write
  if f then
    function write(s) f:write(s) f:write("\n") end
  else
    function write(s) print(s) end
  end

  ---@param node Lang
  ---@param prefix string?
  local function walk(node, prefix)
    local keys = table.keys(node) ---@type string[]
    table.sort(keys)

    for _, k in ipairs(keys) do
      local v = node[k]
      local pfx = k
      if prefix then
        pfx = prefix .. "." .. pfx
      end

      if type(v) == "table" then
        walk(v, pfx)
      elseif type(v) == "string" then
        write(pfx .. "=" .. v)
      end
    end
  end

  walk(l)
end
