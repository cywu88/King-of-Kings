--[[
-- 单例类
--]]

local Singleton = Class("Singleton")

local function __init(self)
	assert(rawget(self._class_type, "Instance") == nil, self._class_type.__cname.." to create singleton twice!")
	rawset(self._class_type, "Instance", self)
end

local function __delete(self)
	rawset(self._class_type, "Instance", nil)
end

-- 只是用于启动模块
local function Startup(self)
end

-- 不要重写
local function GetInstance(self)
	if rawget(self, "Instance") == nil then        
		rawset(self, "Instance", self.New())
	end
	assert(self.Instance ~= nil)
	return self.Instance
end


Singleton.__init = __init
Singleton.__delete = __delete
Singleton.Startup = Startup
Singleton.GetInstance = GetInstance


return Singleton