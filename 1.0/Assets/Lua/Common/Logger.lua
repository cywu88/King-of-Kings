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