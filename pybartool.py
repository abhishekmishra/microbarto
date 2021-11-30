import yaml
import PySimpleGUI as sg

default_toolbar_cfg = """
    toolbar:
        location:
            x: 100
            y: 100
        orientation: horizontal
        items:
            - name: btn1
              image: btn1.jpg
              type: button
              action: explorer.exe
              action_type: command
"""

tbcfg = yaml.load(default_toolbar_cfg, Loader=yaml.Loader)
print(tbcfg)

print(yaml.dump(tbcfg))

layout = [
    [sg.Button('Close')]
]
for item in tbcfg['toolbar']['items']:
    layout.insert(0, [sg.Button(item['name'])])

# Create the Window
window = sg.Window(
    'pybartool',
    layout,
    no_titlebar=True,
    grab_anywhere=True,
    location=(tbcfg['toolbar']['location']['x'], tbcfg['toolbar']['location']['y']),
    keep_on_top=True
)
# Event Loop to process "events" and get the "values" of the inputs
while True:
    event, values = window.read()
    if event == sg.WIN_CLOSED or event == 'Close':  # if user closes window or clicks cancel
        break

window.close()
