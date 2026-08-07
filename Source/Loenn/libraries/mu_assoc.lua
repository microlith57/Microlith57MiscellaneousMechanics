return function(mu)
  --[[
    associatedMods builder.

    usage:
    > mu.assoc {}             --> {"Microlith57MiscellaneousMechanics"}
    > mu.assoc {expr = true}  --> {"Microlith57MiscellaneousMechanics", "FrostHelper"}
    > mu.assoc {self = false} --> {}
  ]]

  local associations = {
    {"Microlith57MiscellaneousMechanics", self = true},
    {"FrostHelper", expr = true},
  }

  ---@param tbl {[string]: boolean?}
  function mu.assoc(tbl)
    if tbl.self == nil then tbl.self = true end
    local result = {} ---@type string[]
    for _, a in ipairs(associations) do
      local found = false
      for k, _ in pairs(a) do
        if type(k) == "string" and tbl[k] then
          found = true
          break
        end
      end
      if found then table.insert(result, a[1]) end
    end
    return result
  end
end
