import yaml
import PySimpleGUI as sg
import os, os.path
from pathlib import Path

load_default_always = False

#TODO: load theme from config
sg.theme('DarkBrown4')

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
pybartool_cfg_path = os.path.join(home, '.pybartool')

if not load_default_always and os.path.exists(pybartool_cfg_path):
    print(pybartool_cfg_path + ' exists.')
    with open(pybartool_cfg_path, "r") as stream:
        try:
            tbcfg = yaml.safe_load(stream)
        except yaml.YAMLError as exc:
            print(exc)
else:
    print(pybartool_cfg_path + ' doesn\'t exist, loading default.')
    tbcfg = yaml.load(default_toolbar_cfg, Loader=yaml.Loader)

print(tbcfg)
# print(yaml.dump(tbcfg))

layout = [
    [sg.Button('Close')]
]

items = tbcfg['toolbar']['items']
for item_name in items:
    item = items[item_name]
    b = sg.Button(item['name'], k=item_name)
    if tbcfg['toolbar']['orientation'] == 'horizontal':
        layout[0].insert(0, b)
    else:
        layout.insert(0, [b])

# Create the Window
window = sg.Window(
    'pybartool',
    layout,
    no_titlebar=True,
    grab_anywhere=True,
    location=(tbcfg['toolbar']['location']['x'],
              tbcfg['toolbar']['location']['y']),
    margins=(5, 0),
    element_padding=(0, 0),
    keep_on_top=True
)
# Event Loop to process "events" and get the "values" of the inputs
while True:
    event, values = window.read()
    if event == sg.WIN_CLOSED or event == 'Close':  # if user closes window or clicks cancel
        break
    print(event)
    if event in items:
        print(event + ' is a toolbar button.')
        item = items[event]
        if item["action_type"] == "command":
            os.system(item["action"])
        if item["action_type"] == "file":
            try:
                os.startfile(item["action"])
            except FileNotFoundError as fnfe:
                sg.popup_error("File could not be found: " + fnfe.filename)

window.close()
