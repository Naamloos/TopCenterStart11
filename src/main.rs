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

    // define strings for the class name and window name for the start menu
    let class_name : CString = CString::new("Windows.UI.Core.CoreWindow").expect("Whope");
    let window_name : CString = CString::new("Start").expect("Whope");

    unsafe
    {
        // Grab the window handle to the start menu
        let wnd : HWND = winuser::FindWindowA(class_name.as_ptr(), window_name.as_ptr());

        // initialize the DEVMODEA struct. For sonme reason, default still does not work.
        let mut dev_mode = zeroed();
        // Get the current main monitor settings (same monitor start should display on).
        winuser::EnumDisplaySettingsA(std::ptr::null(), winuser::ENUM_CURRENT_SETTINGS, &mut dev_mode);

        // Calculate proper x position and move start menu
        let x_pos = ((dev_mode.dmPelsWidth / 2) - (666 / 2)) as i32;
        winuser::SetWindowPos(wnd, winuser::HWND_TOPMOST, x_pos, 48, 666, 750, 0);
        // Start menu is 666 by 750 pixels (including the padding which is part of the actual window)

        println!("Moved start menu to x:{} y:48", x_pos);

        println!("Activating thumbnail position loop");

        let timeout = time::Duration::from_millis(100);
        let thumbnail_class : CString = CString::new("TaskListThumbnailWnd").expect("Whope");

        // There's a window that has the same class, but the one we want has no title.
        let desktop_switcher_class : CString = CString::new("XamlExplorerHostIslandWindow").expect("Whope");
        let desktop_switcher_name : CString = CString::new("").expect("Whope");

        let thumbnail_wnd : HWND = winuser::FindWindowA(thumbnail_class.as_ptr(), std::ptr::null());
        let mut thumbnail_window_rect : RECT = zeroed();
        let mut thumbnail_client_rect : RECT = zeroed();
        loop
        {
            let desktop_switcher_wnd : HWND = winuser::FindWindowA(desktop_switcher_class.as_ptr(), desktop_switcher_name.as_ptr());
            if !desktop_switcher_wnd.is_null()
            {
                // height 196
                // width dev_mode.dmPelsWidth
                let width = dev_mode.dmPelsWidth as i32;
                winuser::SetWindowPos(desktop_switcher_wnd, winuser::HWND_TOP, 0, 48, width, 196, 0);
            }

            // Get window and client rect for thumbnails
            winuser::GetWindowRect(thumbnail_wnd, &mut thumbnail_window_rect);
            winuser::GetClientRect(thumbnail_wnd, &mut thumbnail_client_rect);
            // set new position
            if thumbnail_window_rect.top != 48 // 48 is the height of the taskbar
            {
                // if the y pos changed, we can be sure the window moved.
                winuser::SetWindowPos(thumbnail_wnd, winuser::HWND_TOP, thumbnail_window_rect.left, 48, thumbnail_client_rect.right, thumbnail_client_rect.bottom, 0);
            }
            // wait before next check
            thread::sleep(timeout);
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