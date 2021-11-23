use std::mem::zeroed;
use std::ffi::CString;

use winapi::shared::windef::HWND;
use winapi::um::winuser;

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
    }
}
