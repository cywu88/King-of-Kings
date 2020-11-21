print("bootstrap hero lua")

local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject


local log = require 'Common.Logger'
local util = require 'XLua.common.util'

local m = {}

local initUIRoot = function()
	local uiroot = GameObject('UIRoot')
  GameObject.DontDestroyOnLoad(uiroot)
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