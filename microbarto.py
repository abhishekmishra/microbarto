#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
microbarto.py

Date: 05/Jan/2022
Author: Abhishek Mishra <abhishekmishra3@gmail.com>

A simple toolbar application for the desktop built using
the excellent PySimpleGUI library.

(c) 2022 Abhishek Mishra, License GPLv3

This program is free software: you can redistribute it
and/or modify it under the terms of the GNU General
Public License as published by the Free Software
Foundation, either version 3 of the License, or (at
your option) any later version.

This program is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the
implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE. See the GNU General Public
License for more details.

You should have received a copy of the GNU General Public
License along with this program.
If not, see <https://www.gnu.org/licenses/>.
"""

__version__ = "0.0.1a0"
__author__ = "Abhishek Mishra"

import yaml
import PySimpleGUI as sg
import os
from pathlib import Path
from functools import partial
import subprocess

PROJECT_HOME = "https://github.com/abhishekmishra/microbarto"
PROGRAM_NAME = "MicroBarto"
PROGRAM_VERSION = __version__
PROGRAM_DESCRIPTION = """
{} {}: Configurable toolbar for the desktop.

(c) 2022 {}
License: GPLv3
""".format(
    PROGRAM_NAME, PROGRAM_VERSION, __author__
)

load_default_always = False

default_toolbar_cfg = """
    toolbar:
        anchor: n
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
    # print(microbarto_cfg_path + " exists.")
    with open(microbarto_cfg_path, "r") as stream:
        try:
            tbcfg = yaml.safe_load(stream)
        except yaml.YAMLError as exc:
            print(exc)
else:
    #print(microbarto_cfg_path + " doesn't exist, loading default.")
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
TBButton = partial(sg.Button, font=tbfont, pad=((0, 0), (0, 0)))

layout = []


def add_tb_button(btn):
    if tbcfg["toolbar"]["orientation"] == "horizontal":
        if len(layout) == 0:
            layout.append([])
        layout[0].insert(0, btn)
    else:
        layout.insert(0, [btn])


add_tb_button(TBButton("✕", k="Close", pad=((0, 0), (0, 0))))
add_tb_button(TBButton("🏠", k="MicroBartoWebsite", pad=((1, 0), (0, 0))))

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
    # grab_anywhere=True,
    location=(
        tbcfg["toolbar"]["location"]["x"],
        tbcfg["toolbar"]["location"]["y"],
    ),
    margins=(0, 0),
    element_padding=(0, 0),
    finalize=True,
    keep_on_top=True,
    right_click_menu=["ignored", ["---", "About MicroBarto", "Exit"]],
)

window.bind("<Enter>", "+MOUSE OVER+")
window.bind("<Leave>", "+MOUSE AWAY+")

window_size = window.size
hidden_window_size = (window_size[0], 2)
window.size = hidden_window_size


def mouse_oob():
    """
    Checks if the mouse location is within pad pixels of the window
    on the y-axis. If it is then it is not yet out of bounds.
    This allows some delay in hiding based on mouse away.
    """
    pad = 3
    loc = window.current_location()
    sz = window.size
    mloc = window.mouse_location()
    if (loc[1] - pad) < mloc[1] and (loc[1] + sz[1] + pad) > mloc[1]:
        return False
    else:
        return True


if __name__ == "__main__":
    while True:
        event, values = window.read()
        if event in (sg.WIN_CLOSED, "Close", "Exit"):
            break
        if event == "About MicroBarto":
            sg.popup_ok(
                PROGRAM_DESCRIPTION,
                title="About " + PROGRAM_NAME + " " + PROGRAM_VERSION,
            )
        if event == "+MOUSE OVER+" and window.size != window_size:
            window.size = window_size
        if event == "+MOUSE AWAY+" and window.size != hidden_window_size:
            if mouse_oob():
                window.size = hidden_window_size
        if event == "MicroBartoWebsite":
            os.startfile(PROJECT_HOME)
        if event in items:
            item = items[event]
            if item["action_type"] == "command":
                subprocess.call([item["action"]])
            if item["action_type"] == "file":
                try:
                    os.startfile(item["action"])
                except FileNotFoundError as fnfe:
                    sg.popup_error("File could not be found: " + fnfe.filename)

    window.close()
