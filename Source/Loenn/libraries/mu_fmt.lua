return function(mu)
  --[[
    string formatter.

    usage:
    > local f = mu.fmt { example = "a", another = "b" }
    > f"{example}, {another}" --> "a, b"

    a more complicated example, using mu.vary:
    > local channels = mu.vary {
    >   col = {"r", "g", "b", "a"},
    >   name = {"red", "green", "blue", "premultiplied alpha"},
    > }
    >
    > local res = {}
    > local f = mu.fmt {Noun = "Expression"}
    > for _, c in ipairs(channels) do
    >   c(f) -- merge the contents of f into c
    >   res[c.col] = c"{Noun} for the {name} component."
    > end

    yields:
    > {
    >   r = "Expression for the red component."
    >   g = "Expression for the green component."
    >   b = "Expression for the blue component."
    >   a = "Expression for the premultiplied alpha component."
    > }
  ]]

  ---@class Fmt
  ---@field [any] any
  ---@operator call(string): string
  local Fmt = {}

  ---@overload fun(self, o: string): string
  ---@overload fun(self, o: table): table
  function Fmt:__call(o)
    if type(o) == "string" then
      return self:_format(o)
    elseif type(o) == "table" then
      return self:_merge(o)
    end
    return o
  end
  function Fmt:__index(key)
    local meta = rawget(Fmt, key)
    if meta then return meta end

    if type(key) ~= "string" then return end

    local upper = false
    key = key:gsub("%a", function(a)
      if a:match("%u") then upper = true end
      return a:lower()
    end, 1)

    local a = false
    key = key:gsub("^an? ", function()
      a = true;
      return ""
    end)

    local val = rawget(self, key)
    if not val then return end
    val = tostring(val)

    if a then
      local vowel = val:match("^[aeiou]")
      val = (vowel and "an " or "a ") .. val
    end

    if upper then
      val = val:gsub("%l", string.upper, 1)
    end

    return val
  end
  local punct_precedence = {[","] = 1, [";"] = 2, ["-"] = 3}
  ---@private
  ---@param s string
  function Fmt:_format_one(s)
    local open = s:match("^%s*[%(%)%[%]]*") or "" ---@type string
    local close = s:match("[%(%)%[%]]*%s*$") or "" ---@type string
    local c = s:sub(#open + 1, -1 - #close)

    local res = {} ---@type string[]
    local i = 1
    local punct = ""
    while true do
      local next = c:find("[,;-]", i)
      local j = next and next - 1 or #c
      local key = c:sub(i, j):gsub("^%s+", ""):gsub("%s+$", "")
      local val = self[key]
      if val == nil then error(("key %s not found in format string"):format(key)) end

      local found = val ~= ""
      if found then
        local p = punct
        if punct ~= "" then p = p .. " " end
        if punct == "-" then p = " " .. p end
        if #res > 0 then table.insert(res, p) end
        table.insert(res, val)
      end

      i = j + 2
      if i > #c then break end

      local next_punct = next and c:sub(next, next) or ""
      local prev_prec = punct_precedence[punct] or -2
      local next_prec = punct_precedence[next_punct] or -1
      if found or next_prec > prev_prec then
        punct = next_punct
      end
    end
    if #res > 0 then
      return open .. table.concat(res) .. close
    else
      return ""
    end
  end
  ---@private
  ---@param s string
  function Fmt:_format(s)
    local res = {} ---@type string[]
    local i = 1
    while true do
      local open = s:find("{", i)
      if not open then
        local t = s:sub(i):gsub("}}", "}")
        table.insert(res, t)
        break
      elseif s[open + 1] == "{" then
        local t = s:sub(i, open - 1)
        table.insert(res, t)
        table.insert(res, "{")
        i = open + 2
      else
        if open > i then
          local t = s:sub(i, open - 1):gsub("}}", "}")
          table.insert(res, t)
        end
        local close = s:find("}", open + 1)
        if not close then error(("unbalanced curly braces in format string, starting at %d"):format(open)) end
        if open == close then error(("empty format specifier at %d"):format(open)) end
        local contents = s:sub(open + 1, close - 1)
        table.insert(res, self:_format_one(contents))
        i = close + 1
      end
    end
    return table.concat(res)
  end
  ---@private
  ---@param t table
  function Fmt:_merge(t)
    for k, v in pairs(t) do self[k] = v end
    return self
  end

  ---@param t table
  ---@return Fmt
  function mu.fmt(t)
    setmetatable(t, Fmt)
    return t
  end

  ---@param tbl {[string]: any[]}
  function mu.vary(tbl)
    local result = {} ---@type Fmt[]
    local _, ref = next(tbl)
    if not ref then error("must have at least one variant!") end
    for i = 1, #ref do
      local r = mu.fmt {}
      for k, v in pairs(tbl) do
        r[k] = v[i]
      end
      table.insert(result, r)
    end
    return result
  end
end
