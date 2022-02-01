--[[

microbarto.lua: utility module defining functions for creating
    and customizing toolbar items.

Author: Abhishek Mishra
Date: 01/02/2022

--]]

local _lua_print = print
function print(...)
	if mb ~= nil then
		mb:print(...)
	else
		_lua_print(...)
	end
end

local _mbobj_functions_names = {
    "AddBtn",
    "LaunchUrl"
}

-- For each function in the function names list
-- create a corresponding global function which calles the
-- function on the global mb object
for k, v in ipairs(_mbobj_functions_names) do
	_G[v] = function (...) mb[v](mb, ...) end
end

function AddUrlBtn(label, url)
    AddBtn(label, function (sender, event)
        LaunchUrl(url)
    end)
end
