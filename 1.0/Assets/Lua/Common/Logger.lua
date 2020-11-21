local Logger = {}
local function Log(msg)
	if GameSetting.Debug then
		print(debug.traceback(msg, 2))
	else
		CS.Logger.Log(debug.traceback(msg, 2))
	end
end

local function LogError(msg)
	if GameSetting.Debug then
		error(msg, 2)
	else
		CS.Logger.LogError(debug.traceback(msg, 2))
	end
end

Logger.Log = Log
Logger.LogError = LogError

--[[
	方便打印多个值，用 | 分割
]]
local Time = Time or CS.UnityEngine.Time
local function dump(...)
	local tbl = table.pack(...)
	local out = {Time.frameCount}
	for i = 1, tbl.n do
		table.insert(out, tostring(tbl[i]))
	end
	return table.concat(out, ' | ')
end

function Logger.d(...)
	CS.Logger.Log(debug.traceback(dump(...), 2))
end
function Logger.e(...)
	CS.Logger.LogError(debug.traceback(dump(...), 2))
end
function Logger.color(color, ...)
	local s = string.format('<color=%s>%s</color>', color, dump(...))
	CS.Logger.Log(debug.traceback(s, 2))
end

return Logger