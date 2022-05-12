#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

extern crate single_instance;

use single_instance::SingleInstance;

use std::mem::zeroed;
use std::ffi::CString;
use std::{thread, time, process};

use winapi::shared::windef::HWND;
use winapi::um::winuser;
use winapi::shared::windef::RECT;
use winapi::ctypes::c_void;

// ClassName and Window title for the start menu
const START_MENU_CLASS:&str = "Windows.UI.Core.CoreWindow";
const START_MENU_TITLE:&str = "Start";

// ClassName for the thumbnail. Has a null title
const THUMBNAIL_CLASS:&str = "TaskListThumbnailWnd";

// ClassName and Window title for the desktop switcher
const DESKTOP_SWITCHER_CLASS:&str = "XamlExplorerHostIslandWindow";
const DESKTOP_SWITCHER_TITLE:&str = "";

// ClassName for taskbar
const TASKBAR_CLASS:&str = "Shell_TrayWnd";

// Width, height values
const START_WIDTH:i32 = 666;
const START_HEIGHT:i32 = 750;
const TASKBAR_HEIGHT:i32 = 48;

// Rate at which the program does its polling. default is 250.
const POLLING_RATE:u64 = 1; // TODO: make configurable?

fn main() 
{

    let instance = SingleInstance::new("TopCenterStart11").unwrap();
    if !instance.is_single() {
        process::exit(0x0100);
    }
    
    println!("Let's fix what Microsoft couldn't!");

    unsafe
    {
        // Grab the window handle to the start menu
        let wnd : HWND = find_hwnd(START_MENU_CLASS, Some(START_MENU_TITLE));

        // initialize the DEVMODEA struct. For sonme reason, default still does not work.
        let mut dev_mode = zeroed();
        // Get the current main monitor settings (same monitor start should display on).
        winuser::EnumDisplaySettingsA(std::ptr::null(), winuser::ENUM_CURRENT_SETTINGS, &mut dev_mode);

        println!("Starting placement-fix loop");
        // Define a timeout
        let timeout = time::Duration::from_millis(POLLING_RATE);
        // Pre-fetch thumbnail wnd. This shouldn't change during the OS lifetime.
        let thumbnail_wnd : HWND = find_hwnd(THUMBNAIL_CLASS, None);
        /*
        DesktopWindowXamlSource
        Start
        System Promoted Notification Area
        User Promoted Notification Area
        Tray Input Indicator
        DesktopWindowXamlSource
        Running applications
        Running applications
        DesktopWindowXamlSource
        */

        let mut work_rect : RECT = zeroed();
        let lparam = &work_rect as *const RECT as *mut c_void;
        winuser::SystemParametersInfoA(winuser::SPI_GETWORKAREA, 0, lparam, 0);
        work_rect.top = work_rect.top + 48;
        work_rect.bottom = work_rect.bottom + 48;

        let taskbar_wnd : HWND = find_hwnd(TASKBAR_CLASS, None);
        let mut tbwindow_rect : RECT = zeroed();
        let mut tbclient_rect : RECT = zeroed();
    
        winuser::GetWindowRect(taskbar_wnd, &mut tbwindow_rect);
        winuser::GetClientRect(taskbar_wnd, &mut tbclient_rect);

        loop
        {
            // if the y pos changed, we can be sure the window moved.
            winuser::SetWindowPos(taskbar_wnd, std::ptr::null_mut(), tbwindow_rect.left, 0, tbclient_rect.right, tbclient_rect.bottom, winuser::SWP_NOSENDCHANGING);
            winuser::ShowWindow(taskbar_wnd, winuser::SW_SHOW);
            winuser::UpdateWindow(taskbar_wnd);
            winuser::SystemParametersInfoA(winuser::SPI_SETWORKAREA, 0, lparam, winuser::SPIF_SENDCHANGE);

            // grab desktop switcher wnd and check whether it exists.
            let desktop_switcher_wnd : HWND = find_hwnd(DESKTOP_SWITCHER_CLASS, Some(DESKTOP_SWITCHER_TITLE));
            place_window_under_taskbar(desktop_switcher_wnd, TASKBAR_HEIGHT);

            // correctly place thumbnail window
            place_window_under_taskbar(thumbnail_wnd, TASKBAR_HEIGHT);

            place_window_under_taskbar(taskbar_wnd, 0);

            // Calculate proper x position and move start menu
            let x_pos = ((dev_mode.dmPelsWidth / 2) - (START_WIDTH as u32 / 2)) as i32;
            winuser::SetWindowPos(wnd, winuser::HWND_TOPMOST, x_pos, TASKBAR_HEIGHT, START_WIDTH, START_HEIGHT, 0);

            // These should be the notification panel and control panel, but IO can't get them to work as of right now.
            // let control_wnd : HWND = find_hwnd("Windows.UI.Core.CoreWindow", Some("Control Center"));
            // let notif_wnd : HWND = find_hwnd("Windows.UI.Core.CoreWindow", Some("Notification Center"));
            // place_window_under_taskbar(control_wnd, 48);
            // place_window_under_taskbar(notif_wnd, 48);
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
        if window_rect.top != pos
        {
            // if the y pos changed, we can be sure the window moved.
            winuser::SetWindowPos(window, winuser::HWND_TOP, window_rect.left, pos, client_rect.right, client_rect.bottom, 0);
        }
    }
}
