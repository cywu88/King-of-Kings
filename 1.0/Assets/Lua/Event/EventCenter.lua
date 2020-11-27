local EventCenter = Class("EventCenter",Singleton)

local function __init(self)
	--print("******************************************  EventCenter Init  " ..tostring(self))
	self.event_num = 0
	self.event_id = 0
	self.event_map = {}
	self.event_id_map = {}
end

local function __delete(self)
	self.event_map = nil
	self.event_id_map = nil
end


local function __getEventNum(self)
	return self.event_num
end

local function __add(self,global_event_id, func)
	local ev = self.event_map[global_event_id]
	if not ev then
		self.event_id = self.event_id + 1
		ev = {}
		ev.event_id = self.event_id
		ev.ev_handle_id = 0
		ev.function_list = {}
		self.event_map[global_event_id] = ev
		self.event_id_map[self.event_id] = ev
	end

	self.event_num = self.event_num + 1
	ev.ev_handle_id = ev.ev_handle_id + 1
	ev.function_list[ev.ev_handle_id] = func

	local ev_handle = ev.event_id + ev.ev_handle_id * 10000
	return ev_handle
end

local function __remove(self,ev_handle)
	if not ev_handle then
		return
	end
	local event_id = ev_handle % 10000
	local handle_id = math.floor(ev_handle / 10000)

    --Logger.Log("ev_handle "..tostring(ev_handle).." remove id "..tostring(event_id) .. " handle id :"..tostring(handle_id))

	local ev = self.event_id_map[event_id]
	if not ev then
		error("EventCenter:移除的事件 没有找到event_id!", event_id)
		return
	end

	if not ev.function_list[handle_id] then
		error("EventCenter:移除的事件 没有找到handle_id!", event_id, handle_id)
		return
	end

	self.event_num = self.event_num - 1
	ev.function_list[handle_id] = nil
end

local function __fire(self,global_event_id, ...)
	--print("******************************************  EventCenter __fire  " ..tostring(self))
	local ev = self.event_map[global_event_id]
	if not ev then
		return
	end

	for i,v in pairs(ev.function_list) do
		if v then
			v(...)
		end
	end
end

EventCenter.__init = __init
EventCenter.__delete = __delete
EventCenter.Add = __add
EventCenter.Remove = __remove
EventCenter.Fire = __fire
EventCenter.GetEventNum = __getEventNum

return EventCenter