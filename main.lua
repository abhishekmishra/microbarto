function rtrim(x)
	return x:match('(.-)%s*$')
end

while 1 do
	io.write("Yo> ")
	input = ''
	repeat
		line = io.read()
		line = rtrim(line)
		input = input .. line .. '\n'
	until (line:sub(-1) ~= '\\')
	io.write(input)
	if rtrim(input) == 'quit' then
		break
	end
end
