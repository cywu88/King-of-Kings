print("bootstrap hero lua")

local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject

local CSResMgr = CS.ResourceMgr_New.Instance

local log = require 'Common.Logger'
local respath = require 'Common.respath'
local util = require 'XLua.common.util'

local m = {}
local RESOLUTION = {1366, 768}

local initUIRoot = function()
	local uiroot = GameObject('UIRoot')
  GameObject.DontDestroyOnLoad(uiroot)

  local uicamera = CSResMgr:CreateGameObjectFromResources(respath.builtin.uicamera)
  uicamera.name = 'UICamera'
  uicamera.transform:SetParent(uiroot.transform, false)

  local canvas = uiroot:AddComponent(typeof(UnityEngine.Canvas))
  canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceCamera
  canvas.worldCamera = uicamera:GetComponent(typeof(UnityEngine.Camera))
  canvas.planeDistance = 1000

  local scaler = uiroot:AddComponent(typeof(UnityEngine.UI.CanvasScaler))
  scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize
  scaler.referenceResolution = UnityEngine.Vector2(RESOLUTION[1], RESOLUTION[2])
  local scaleX = UnityEngine.Screen.width / RESOLUTION[1]
  local scaleY = UnityEngine.Screen.height / RESOLUTION[2]
  scaler.matchWidthOrHeight = scaleX > scaleY and 1 or 0

  local goevent = GameObject('EventSystem')
  goevent:AddComponent(typeof(UnityEngine.EventSystems.EventSystem))
  goevent:AddComponent(typeof(UnityEngine.EventSystems.StandaloneInputModule))
  GameObject.DontDestroyOnLoad(goevent)


end

local loadLogin = function()
	 log.d('loadLogin...')
end

local initUI = function()
  log.d('initUI')
  initUIRoot()
  loadLogin()
end

local wrapstep = function(fn, desc)
  return function(...)
    local ok, err = pcall(fn, ...)
    if not ok then
      local msg = 'BUILTIN_TEXT.UNKNOWN_ERROR' .. ': ' .. desc
      log.e(msg, err)
      coroutine.yield()
    end
  end
end

local start = function()
  log.d('------------------', 'bootstrap_1 start')

  local steps = {
    initUI,
  }

  for k, v in ipairs(steps) do
    local fn = wrapstep(v, k)
    fn()
  end
  log.d('------------------', 'bootstrap_1 done')

  m._mono.NeedDisplose = true
end

m.start = util.coroutine_call(start)
m.update = function()
  -- uilogin.update()
end

return m