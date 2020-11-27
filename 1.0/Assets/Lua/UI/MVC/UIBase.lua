local UIBase = Class("UIBase")

local event_center = EventCenter:GetInstance()

local function __addEvent(self,event_id, func)
	local ev_handle = event_center:Add(event_id, func)
	self._bind_event_dic = self._bind_event_dic or {}
	self._bind_event_dic[ev_handle] = true
	return ev_handle
end

local function __removeEvent(self,ev_handle)
	self._bind_event_dic[ev_handle] = nil
	event_center:Remove(ev_handle)
end

local function __removeAllEvents(self)
	if self._bind_event_dic then
		for ev_handle,_ in pairs(self._bind_event_dic) do
			event_center:Remove(ev_handle)
		end
	end
	self._bind_event_dic = nil
end


local function __addClickEventListenerEx(self,click_obj, func, click_audio_type)
    click_obj:AddClickEventListener(function (x, y)
    	-- 通用点击音效
        Game.AudioMgr:PlaySound(EAudioID.CommonClick, AudioState.Play, nil)
		func(x, y)
	end)
end

local function __addToggleEventListenerEx(self,click_obj, func, click_audio_type)
    click_obj:AddToggleEventListener(function (x, y)
    	-- 通用页签音效
    	Game.AudioMgr:PlaySound(EAudioID.LargeToggle, AudioState.Play, nil)
		func(x, y)
	end)
end

local function __addDropDownEventListenerEx(self,click_obj,func,click_audio_type)
    click_obj:AddDropDownEventListener(function (index)
        func(index)
    end)
end

local function __addDoubleClickEventListenerEx(self,click_obj,func,click_audio_type)
	click_obj:AddDoubleClickEventListener(function ()
        func()
    end)
end

local function __init(self)
end

local function __delete(self)
    __removeAllEvents(self)
end


UIBase.__init = __init
UIBase.__delete = __delete
UIBase.AddEvent = __addEvent
UIBase.RemoveEvent = __removeEvent
UIBase.RemoveAllEvents = __removeAllEvents
UIBase.AddClickEventListenerEx = __addClickEventListenerEx
UIBase.AddToggleEventListenerEx = __addToggleEventListenerEx
UIBase.AddDropDownEventListenerEx = __addDropDownEventListenerEx
UIBase.AddDoubleClickEventListenerEx = __addDoubleClickEventListenerEx


return UIBase


