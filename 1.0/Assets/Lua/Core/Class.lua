--保存类类型的虚表
local _class = {}

-- 自定义类型
ClassType = {
	class = 1,
	instance = 2
}

function Class(classname,super)
	assert(type(classname) == 'string' and #classname > 0)
	-- 生成一个类类型
	local class_type = {}

	-- 在创建对象的时候自动调用
	class_type.__init = false
	class_type.__delete = false
	class_type.__cname = classname
	class_type.__ctype = ClassType.class

	class_type.super = super
	class_type.New = function (...)
		-- 生成要给对象
		local obj = {}
		obj.__class_type = class_type
		obj.__ctype = ClassType.instance

		--
		setmetatable(
			obj,
			{
				__index = _class[class_type]
			}
		)
		--调用初始化方法
		do
			local create 
			create = function(c,...)
				if c.super then
					create(c.super,...)
				end
				if c.__init then
					c.__init(obj,...)
				end
			end

			create(class_type,...)
		end

		obj.Delete = function(self)
			local now_super = self.__class_type
			while now_super do
				if now_super.__delete then
					now_super.__delete(self)
				end
				now_super = now_super.super
			end
		end

		return obj
	end

	local vtabl = {}
	_class[class_type] = vtabl
	setmetatable(
		class_type,
		{
			__newindex = function (t,k,v)
				vtbl[k] = v
			end
			__index = vtbl
		}
	)

	if super then
		setmetatable(
			vtbl,
			{
				__index = function(t,k)
					local ret = _class[super][k]
					vtbl[k] = ret
					return ret
				end
			}
		)
	end

	return class_type
end


function new(p,...)
	p = p or {}
	local t = setmetatable({},{
		__index = p,
	})
	if t.__init then
		t:init(...)
	end
	return t
end