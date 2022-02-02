--[[

mbconfig.lua: use this file to customize the microbarto toolbar.
    The sample config file contains a few commented out examples
    to help you get started.

The following are available in global scope:
    mb: this is the object representing the toolbar main window
        and provides utility functions to add and configure
        toolbar buttons.

Author: Abhishek Mishra
Date: 01/02/2022
License: GPLv3

--]]

-- Following 3 examples all create a button which opens a url
-- since the methods from mb are also available in
-- global scope, it is easier to use them directly.

mb:AddBtn("Google", function (sender, event)
    mb:LaunchUrl("https://google.com")
end)

AddBtn("Yahoo", function (sender, event)
    mb:LaunchUrl("https://yahoo.com")
end)

AddUrlBtn("Bing", "https://bing.com")
AddProgramBtn("FM", "explorer.exe", "C:\\")
