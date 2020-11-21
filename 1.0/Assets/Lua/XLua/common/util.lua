-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

local pack = table.pack
local unpack = function(t, i, j) return table.unpack(t, i or 1, j or t.n or #t) end

local tinsert = function(tbl, idx, v)
    local sz = tbl.n or #tbl
    for i = sz+1, idx+1, -1 do
        tbl[i] = tbl[i-1]
    end
    tbl[idx] = v
    if tbl.n then
        tbl.n = tbl.n + 1
    end
end

local function async_to_sync(async_func, callback_pos)
    return function(...)
        local _co = coroutine.running() or error ('this function must be run in coroutine')
        local rets
        local waiting = false
        local function cb_func(...)
            if waiting then
                assert(coroutine.resume(_co, ...))
            else
                rets = {...}
            end
        end
        local params = pack(...)
        tinsert(params, callback_pos or (#params + 1), cb_func)
        async_func(unpack(params))
        if rets == nil then
            waiting = true
            rets = {coroutine.yield()}
        end
        
        return unpack(rets)
    end
end

local function coroutine_call(func)
    return function(...)
        local co = coroutine.create(func)
        assert(coroutine.resume(co, ...))
    end
end

return {
    async_to_sync = async_to_sync,
    coroutine_call = coroutine_call,
}