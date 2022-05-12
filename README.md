# TopCenterStart11
TopCenterStart11 is small utility that moves the start menu and Taskbar to the top-center of the screen in Windows 11.

It also fixes the issue that in a stock Windows 11 install, when the taskbar is hacked to the top of the screen, the preview windows that pop up when you hover over a program icon pops up ABOVE the top of the screen. That means they cannot be seen or clicked, and a minimized window cannot be re-opened. This utility causes those to show ON screen, restoring functionality.

## 📃 How to use?
TopCenterStart11 changes some behavior of the Taskbar and it's components. To consistently launch the application at bootup, some configuration has to be done. Please read the following carefully:

1. Download the latest version from the [Releases](https://github.com/Naamloos/TopCenterStart11/releases) page.
2. Place the EXE at any location on your system. Preferrably it's own dedicated folder.
3. Launch the application.
4. Double click the icon that appeared in your system tray.
5. Click "Enable Autostart".
6. Profit!

Now, the application should run at startup. Have fun 😊💕

## 💕 Donations
If this application has been useful to you in any way whatsoever, feel free to buy me a coffee!

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/V7V09Q1I)

## ⭐ Other software that works well with TopCenterStart11
### Windows11DragAndDropToTaskbarFix
Download: https://github.com/HerMajestyDrMona/Windows11DragAndDropToTaskbarFix

**NOTE**: When the taskbar is on top, the Windows11DragAndDropToTaskbarFix works better when the following [configuration line](https://github.com/HerMajestyDrMona/Windows11DragAndDropToTaskbarFix/blob/main/CONFIGURATION.md) is added:
```
DetectKnownPixelColorsToPreventAccidentalEvents=0
```
Extra details: https://github.com/HerMajestyDrMona/Windows11DragAndDropToTaskbarFix/issues/70#issuecomment-1061368908

💫 Special thanks to [HerMajestyDrMona](https://github.com/HerMajestyDrMona) 💫

### Taskbar11
Download: https://github.com/jetspiking/Taskbar11

💫 Special thanks to [jetspiking](https://github.com/jetspiking/) 💫

## 💭 Q & A
### ❓ My taskbar is left/right aligned!
As of right now, this tool only places the start menu at the top of the screen. Adding support for other positions is planned, but as of right now only top-taskbar is supported.

### ❓ My start button is left-aligned!
Then you don't really need this tool, as this matches the default behavior of the start menu

### ❓ The start popup animation has the wrong direction!
I don't think I can actually fix that, but feel free to [prove me wrong](https://github.com/Naamloos/TopCenterStart11/pulls)!

### ❓ I want to stop using the application!
Open settings again, then click "Disable Autostart". After that, right click the tray icon and click Exit. All should go back to normal.

### ❓ The action center isn't vertically aligned right!
I am still looking into a way to fix this. As of right now I have been unable to fix this. [Any help is welcome!](https://github.com/Naamloos/TopCenterStart11/pulls)

## 📸 Screenshot
This is a screenshot of the OLD app, but behavior hasn't changed so this is still accurate.
![](https://i.imgur.com/Ud0IKO2.png)
