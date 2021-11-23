#![windows_subsystem = "windows"] // yeah I know this is a bit weird but it hides the console

use std::mem::zeroed;
use std::ffi::CString;
use std::{thread, time};

use winapi::shared::windef::HWND;
use winapi::um::winuser;
use winapi::shared::windef::RECT;

fn main() 
{
    println!("Let's fix what Microsoft couldn't!");

    unsafe
    {
        // Grab the window handle to the start menu
        let wnd : HWND = find_hwnd("Windows.UI.Core.CoreWindow", Some("Start"));

        // initialize the DEVMODEA struct. For sonme reason, default still does not work.
        let mut dev_mode = zeroed();
        // Get the current main monitor settings (same monitor start should display on).
        winuser::EnumDisplaySettingsA(std::ptr::null(), winuser::ENUM_CURRENT_SETTINGS, &mut dev_mode);

        // Calculate proper x position and move start menu
        let x_pos = ((dev_mode.dmPelsWidth / 2) - (666 / 2)) as i32;
        winuser::SetWindowPos(wnd, winuser::HWND_TOPMOST, x_pos, 48, 666, 750, 0);
        // Start menu is 666 by 750 pixels (including the padding which is part of the actual window)
        // Start needs some extra configuring because it is completely misplaced.
        // The other windows only really need adjustment on their Y-pos
        println!("Moved start menu to x:{} y:48", x_pos);

        println!("Starting placement-fix loop");
        // Define a timeout
        let timeout = time::Duration::from_millis(100);
        // Pre-fetch thumbnail wnd. This shouldn't change during the OS lifetime.
        let thumbnail_wnd : HWND = find_hwnd("TaskListThumbnailWnd", None);

        loop
        {
            // grab desktop switcher wnd and check whether it exists.
            let desktop_switcher_wnd : HWND = find_hwnd("XamlExplorerHostIslandWindow", Some(""));
            place_window_under_taskbar(desktop_switcher_wnd, 48);

            // correctly place thumbnail window
            place_window_under_taskbar(thumbnail_wnd, 48);

            // wait a little bit before next tick
            thread::sleep(timeout);
        }
    }
}

fn find_hwnd(classname : &str, windowname : Option<&str>) -> HWND
{
    unsafe
    {
        let class = CString::new(classname).expect("");
        let window = CString::new(windowname.unwrap_or("")).expect("");

        // Not the most pretty but it IS a fix for None values in window name.
        return match windowname 
        {
            Some(_val) => winuser::FindWindowA(class.as_ptr(), window.as_ptr()),
            None => winuser::FindWindowA(class.as_ptr(), std::ptr::null())
        };
    }
}

fn place_window_under_taskbar(window : HWND, pos : i32)
{
    if window.is_null()
    {
        return; // no handle no action
    }

    unsafe
    {
        let mut window_rect : RECT = zeroed();
        let mut client_rect : RECT = zeroed();

        winuser::GetWindowRect(window, &mut window_rect);
        winuser::GetClientRect(window, &mut client_rect);

        // set new position
        if window_rect.top != pos // 48 is the height of the taskbar
        {
            // if the y pos changed, we can be sure the window moved.
            winuser::SetWindowPos(window, winuser::HWND_TOP, window_rect.left, pos, client_rect.right, client_rect.bottom, 0);
        }
    }
}

/*
    Note to self:
    class name: Windows.UI.Core.CoreWindow
    window name: Notification Center
    has it's own height so moving it to y=48 would be easy enough! :)

    same for:
    class name: Windows.UI.Core.CoreWindow
    window name: Control Center

    Not sure what's going on with the keyboard language switcher but it should be easy enough to figure out.
*/