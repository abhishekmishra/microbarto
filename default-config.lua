--[[

mbconfig.lua: use this file to customize the microbarto toolbar.
    The sample config file contains a few commented out examples
    to help you get started.

The following are available in global scope:
    mb: this is the object representing the toolbar main window
        and provides utility functions to add and configure
        toolbar buttons.
    AddBtn (label, fn): create a button with a label, which triggers a lua
        function
    AddUrlBtn (label, url): create a button with a label that triggers the
        given url
    AddProgramBtn (label, program, ... args): create a button with a label
        that triggers the given program with the supplied args.

Author: Abhishek Mishra

--]]

-- Following 3 examples all create a button which opens a url
-- since the methods from mb are also available in
-- global scope, it is easier to use them directly.

-- Add a button which triggers a function
-- use mb library prefix
mb:AddBtn("Google", function (sender, event)
    mb:LaunchUrl("https://google.com")
end)

-- Add a button which triggers a function
-- by directly using the functions in global namespace
AddBtn("Yahoo", function (sender, event)
    mb:LaunchUrl("https://yahoo.com")
end)

-- Add a button which opens a URL in the default browser
AddUrlBtn("Bing", "https://bing.com")

-- Add a button which triggers a program with supplied arguments
AddProgramBtn("FM", "explorer.exe", "C:\\")
