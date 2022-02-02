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
    "LaunchUrl",
    "LaunchProgram"
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

--[[
AddProgramBtn: create a button which launches a program.

Params:
    label: label of the button
    app: path to the program to be launched
    rest(varargs): arguments to be passed to the app (optional)
--]]
function AddProgramBtn(label, app, ...)
    local f_args = {...}
    AddBtn(label, function (sender, event)
        LaunchProgram(app, table.unpack(f_args))
    end)
end
