import yaml
import PySimpleGUI as sg
import os
from pathlib import Path
from functools import partial

load_default_always = False

default_toolbar_cfg = """
    toolbar:
        location:
            x: 100
            y: 100
        orientation: horizontal
        items:
            btn1:
                name: file manager
                image: btn1.jpg
                type: button
                action: explorer.exe
                action_type: command
            btn2:
                name: open blah.txt
                image: btn2.jpg
                type: button
                action: blah.txt
                action_type: file
"""

home = str(Path.home())
# print ('user home folder is ' + home)
microbarto_cfg_path = os.path.join(home, ".microbarto")

if not load_default_always and os.path.exists(microbarto_cfg_path):
    print(microbarto_cfg_path + " exists.")
    with open(microbarto_cfg_path, "r") as stream:
        try:
            tbcfg = yaml.safe_load(stream)
        except yaml.YAMLError as exc:
            print(exc)
else:
    print(microbarto_cfg_path + " doesn't exist, loading default.")
    tbcfg = yaml.load(default_toolbar_cfg, Loader=yaml.Loader)

# print(yaml.dump(tbcfg))

# Default configurations
tb_theme = "DarkBrown4"

tbfont_family = "Helvetica"
tbfont_size = 14
tbfont_styles = ""

if "theme" in tbcfg["toolbar"]:
    tb_theme = tbcfg["toolbar"]["theme"]

if "font" in tbcfg["toolbar"]:
    if "family" in tbcfg["toolbar"]["font"]:
        tbfont_family = tbcfg["toolbar"]["font"]["family"]
    if "size" in tbcfg["toolbar"]["font"]:
        tbfont_size = tbcfg["toolbar"]["font"]["size"]
    if "styles" in tbcfg["toolbar"]["font"]:
        tbfont_styles = tbcfg["toolbar"]["font"]["styles"]

# Set the pysimplegui theme to use
sg.theme(tb_theme)

# create the font object
tbfont = (tbfont_family, tbfont_size, tbfont_styles)

# create a TBButton class which has the common config for the
# toolbar buttons.
TBButton = partial(sg.Button, font=tbfont)

layout = [[TBButton("X", k="Close", pad=((10, 0), (0, 0)))]]

items = tbcfg["toolbar"]["items"]
for item_name in items:
    item = items[item_name]
    b = TBButton(item["name"], k=item_name)
    if tbcfg["toolbar"]["orientation"] == "horizontal":
        layout[0].insert(0, b)
    else:
        layout.insert(0, [b])

# Create the Window
window = sg.Window(
    "microbarto",
    layout,
    no_titlebar=True,
    grab_anywhere=True,
    location=(
        tbcfg["toolbar"]["location"]["x"],
        tbcfg["toolbar"]["location"]["y"],
    ),
    margins=(5, 0),
    element_padding=(0, 0),
    keep_on_top=True,
)
# Event Loop to process "events" and get the "values" of the inputs
while True:
    event, values = window.read()
    if (
        event == sg.WIN_CLOSED or event == "Close"
    ):  # if user closes window or clicks cancel
        break
    if event in items:
        item = items[event]
        if item["action_type"] == "command":
            os.system(item["action"])
        if item["action_type"] == "file":
            try:
                os.startfile(item["action"])
            except FileNotFoundError as fnfe:
                sg.popup_error("File could not be found: " + fnfe.filename)

window.close()
