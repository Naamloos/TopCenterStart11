# TopCenterStart11
TopCenterStart11 is small utility that moves the start menu and Taskbar to the top-center of the screen in Windows 11.

## 🤔 What does it do?
This tool moves the taskbar to the top of the screen without registry hacks, restoring behavior that has been removed by Microsoft. Along with this, it also fixes some positioning issues that already existed with the registry hack. The start menu, window previews and desktop switcher now appear at the right spot.

## 📃 How to use?
TopCenterStart11 changes some behavior of the Taskbar and it's components. To consistently launch the application at bootup, some configuration has to be done. Please read the following carefully:

1. Ensure you don't have the registry hack enabled (if your OS still supports this).
2. Ensure the [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) is installed. (Pick Windows->x64, as Windows 11 no longer supports x86 as far as I am aware)
3. Download the latest version from the [Releases](https://github.com/Naamloos/TopCenterStart11/releases) page.
4. Place the EXE at any location on your system. Preferrably it's own dedicated folder.
5. Launch the application.
6. Tell the app whether you want to autostart it or not
7. Profit! 🥳
8. (Optional) Right click the tray icon and click `Exit` to exit.

Now, the application should run at startup. Have fun 😊💕

## 💕 Donations
If this application has been useful to you in any way whatsoever, feel free to buy me a coffee!

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/V7V09Q1I)

## ✨ Discord
Feel free to join my personal Discord to discuss the project!

[![Discord](https://discord.com/api/guilds/438803108978753536/embed.png?style=banner2)](https://discord.gg/hMRWUTa)

## 💭 Q & A
### ❓ My working area is all goofed after removing the tool!
A manual restart of explorer.exe via taskmanager or a full reboot should fix it. It's a bit of a bug that I can't really fix easily. It's nothing major and it's not persistent.

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

## ⭐ Other software that works well with TopCenterStart11 ⭐ 
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

### RoundedTB
Download: https://github.com/torchgm/RoundedTB

💫 Special thanks to [torchgm](https://github.com/torchgm/) 💫

### TranslucentTB
Download: https://github.com/TranslucentTB/TranslucentTB

💫 Special thanks to [sylveon](https://github.com/sylveon) 💫
