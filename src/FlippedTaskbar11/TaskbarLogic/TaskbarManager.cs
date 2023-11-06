using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using static TopCenterStart11.TaskbarLogic.WIN32;

namespace TopCenterStart11.TaskbarLogic
{
    internal class TaskbarManager
    {
        public const string START_MENU_CLASS = "Windows.UI.Core.CoreWindow";
        public const string START_MENU_TITLE = "Start";

        public const string THUMBNAIL_CLASS = "TaskListThumbnailWnd";

        public const string DESKTOP_SWITCHER_CLASS = "XamlExplorerHostIslandWindow";
        public const string DESKTOP_SWITCHER_TITLE = "";

        public const string TASKBAR_CLASS = "Shell_TrayWnd";
        public const string SECONDARY_TASKBAR_CLASS = "Shell_SecondaryTrayWnd";

        public const string POPUP_TITLE = "PopupHost";
        public const string POPUP_CLASS = "Xaml_WindowedPopupClass";

        public const string SYSTRAY_OVERFLOW_CLASS = "TopLevelWindowForOverflowXamlIsland";

        private const uint SPIF_SENDWININICHANGE = 2;
        private const uint SPIF_UPDATEINIFILE = 1;
        private const uint SPIF_CHANGE = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;

        public const int START_WIDTH = 666;
        public const int START_HEIGHT = 750;

        public const int TASKBAR_HEIGHT = 48;

        private Config config;

        private IntPtr startHwnd;
        private IntPtr systrayOverflowHwnd;

        private ArrayList thumbnailHwnds;
        private ArrayList taskbars;
        private IntPtr switcherHwnd;

        private DEVMODE devmode;

        private CancellationTokenSource cancellationTokenSource;
        private bool valid = true;
        private bool stopped = false;

        public TaskbarManager(Config config)
        {
            this.config = config;
            this.cancellationTokenSource = new CancellationTokenSource();

            // pre-fetch windows on launch.
            this.startHwnd = FindWindowA(START_MENU_CLASS, START_MENU_TITLE);
            
            this.thumbnailHwnds = new ArrayList();

            foreach (var thumbnailHwnd in EnumerateThumbnails())
            {
                this.thumbnailHwnds.Add(thumbnailHwnd);
            }

            this.systrayOverflowHwnd = FindWindowA(SYSTRAY_OVERFLOW_CLASS, null);

            this.taskbars = new ArrayList();

            foreach (var taskbarItem in EnumerateTaskbars())
            {
                this.taskbars.Add(taskbarItem);
            }
        
            this.switcherHwnd = FindWindowA(DESKTOP_SWITCHER_CLASS, DESKTOP_SWITCHER_TITLE);
            // some of these are not persistent,
            // so they require polling (unfortunately)
            // these are stored class-wide anyway to prevent
            // constant re-allocation whatever

            // -1 is for ENUM_CURRENT_SETTINGS, -2 is for ENUM_REGISTRY_SETTINGS
            EnumDisplaySettings(null, -1, ref this.devmode);
            // obviously we want current settings.
        }

        public void StartTaskbarLoop()
        {
            if (!this.valid)
                throw new Exception("This taskbar manager has already started once. Please construct a new one to restart it.");

            foreach (TaskbarItem taskbar in this.taskbars)
            {
                // update taskbar's monitor working area
                UpdateWorkingArea(
                    taskbar,
                    WorkingAreaShift.Down);
            }

            _ = Task.Run(
                this.doTaskbarLoopAsync,
                this.cancellationTokenSource.Token);
            _ = Task.Run(
                this.doWindowPlacementLoopAsync,
                this.cancellationTokenSource.Token);

            this.valid = false;
        }

        public void StopTaskbarLoop()
        {
            if (!this.stopped && !this.valid)
            {
                try
                {
                    foreach (TaskbarItem taskbar in this.taskbars)
                    {
                        // update taskbar's monitor working area
                        UpdateWorkingArea(
                            taskbar,
                            WorkingAreaShift.Up);

                        // update taskbar window
                        UpdateWindow(taskbar.Handle);
                    }

                    this.cancellationTokenSource.Cancel();
                }
                finally
                {
                    this.restartExplorer();
                }

                this.stopped = true;
            }
        }
        
        private static IEnumerable<TaskbarItem> EnumerateTaskbars()
        {
            var monitors = EnumerateMonitors().ToArray();
            var taskbars = GetWindows(new[] { TASKBAR_CLASS, SECONDARY_TASKBAR_CLASS }, monitors.Length);

            foreach (var (deviceName, deviceInstanceId) in monitors)
            {
                var taskbar = taskbars.FirstOrDefault(x => string.Equals(x.deviceName, deviceName, StringComparison.OrdinalIgnoreCase));
                if (taskbar == default)
                    continue;

                var isPrimary = (taskbar.className == SECONDARY_TASKBAR_CLASS);

                var alignment = GetTaskbarAlignment(taskbar.monitorRect, taskbar.windowRect);
                if (alignment == default)
                    continue;

                yield return new TaskbarItem(
                    deviceInstanceId,
                    taskbar.handle,
                    isPrimary,
                    alignment,
                    taskbarRect: taskbar.windowRect,
                    monitorRect: taskbar.monitorRect,
                    workRect: taskbar.workRect);
            }
        }

        private static IEnumerable<IntPtr> EnumerateThumbnails()
        {
            var monitors = EnumerateMonitors()
                .ToArray();

            var thumbnailHwnd = FindWindowEx(
                IntPtr.Zero,
                IntPtr.Zero,
                THUMBNAIL_CLASS,
                null);
            
            do
            {
                yield return thumbnailHwnd;

                thumbnailHwnd = FindWindowEx(
                    IntPtr.Zero,
                    thumbnailHwnd,
                    THUMBNAIL_CLASS,
                    null);
            }
            while (thumbnailHwnd != IntPtr.Zero);
        }

        private static IEnumerable<(string deviceName, string deviceInstanceId)> EnumerateMonitors()
        {
            var size = (uint)Marshal.SizeOf<DISPLAY_DEVICE>();
            var display = new DISPLAY_DEVICE { cb = size };
            var monitor = new DISPLAY_DEVICE { cb = size };

            for (uint i = 0; EnumDisplayDevices(null, i, ref display, EDD_GET_DEVICE_INTERFACE_NAME); i++)
            {
                if (display.StateFlags.HasFlag(DISPLAY_DEVICE_FLAG.DISPLAY_DEVICE_MIRRORING_DRIVER))
                    continue;

                for (uint j = 0; EnumDisplayDevices(display.DeviceName, j, ref monitor, EDD_GET_DEVICE_INTERFACE_NAME); j++)
                {
                    if (!monitor.StateFlags.HasFlag(DISPLAY_DEVICE_FLAG.DISPLAY_DEVICE_ACTIVE))
                        continue;

                    yield return (display.DeviceName, ConvertDeviceInstanceId(monitor.DeviceID));
                }
            }
        }

        private static (IntPtr handle, string className, RECT windowRect, string deviceName, RECT monitorRect, RECT workRect)[] GetWindows(string[] classNames, int count)
        {
            var windows = new List<(IntPtr, string, RECT, string, RECT, RECT)>();

            if (EnumWindows(
                    Proc,
                    IntPtr.Zero))
            {
                return windows.ToArray();
            }
            return Array.Empty<(IntPtr, string, RECT, string, RECT, RECT)>();

            bool Proc(IntPtr windowHandle, IntPtr _lParam)
            {
                var buffer = new StringBuilder(256);

                if (GetClassName(
                        windowHandle,
                        buffer,
                        buffer.Capacity) > 0)
                {
                    var className = buffer.ToString();
                    if (classNames.Contains(className))
                    {
                        if (GetWindowRect(
                                windowHandle,
                                out RECT windowRect))
                        {
                            var monitorHandle = MonitorFromWindow(
                                windowHandle,
                                MONITOR_DEFAULTTO.MONITOR_DEFAULTTONULL);
                            if (monitorHandle != IntPtr.Zero)
                            {
                                var monitorInfo = new MONITORINFOEX { cbSize = (uint)Marshal.SizeOf<MONITORINFOEX>() };

                                if (GetMonitorInfo(
                                        monitorHandle,
                                        ref monitorInfo))
                                {
                                    windows.Add((windowHandle, className, windowRect, monitorInfo.szDevice, monitorInfo.rcMonitor, monitorInfo.rcWork));
                                    if (windows.Count > count)
                                        return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        private static string ConvertDeviceInstanceId(string devicePath)
        {
            int index = devicePath.IndexOf("DISPLAY", StringComparison.Ordinal);
            if (index < 0)
                return null;

            var fields = devicePath.Substring(index).Split('#');
            if (fields.Length < 3)
                return null;

            return string.Join(@"\", fields.Take(3));
        }

        private static TaskbarAlignment GetTaskbarAlignment(RECT monitorRect, RECT taskbarRect)
        {
            return (left: (monitorRect.Left == taskbarRect.Left),
                       top: (monitorRect.Top == taskbarRect.Top),
                       right: (monitorRect.Right == taskbarRect.Right),
                       bottom: (monitorRect.Bottom == taskbarRect.Bottom)) switch
                {
                    (true, true, right: false, true) => TaskbarAlignment.Left,
                    (true, true, true, bottom: false) => TaskbarAlignment.Top,
                    (left: false, true, true, true) => TaskbarAlignment.Right,
                    (true, top: false, true, true) => TaskbarAlignment.Bottom,
                    _ => default
                };
        }
    
        private void UpdateWorkingArea(TaskbarItem taskbar, WorkingAreaShift shiftDirection)
        {
            var taskbarWorkRect = taskbar.WorkRect;

            switch (shiftDirection)
            {
                case WorkingAreaShift.Down:
                    taskbarWorkRect.Top = TASKBAR_HEIGHT;
                    taskbarWorkRect.Bottom = taskbar.MonitorRect.Bottom;
                    break;
                case WorkingAreaShift.Up:
                    taskbarWorkRect.Top = taskbar.MonitorRect.Top;
                    taskbarWorkRect.Bottom = taskbar.MonitorRect.Bottom - TASKBAR_HEIGHT;
                    break;
            }
        
            SystemParametersInfo(SPI_SETWORKAREA, 0, ref taskbarWorkRect, 0);
        }

        private async Task doTaskbarLoopAsync()
        {
            do
            {
                foreach (TaskbarItem taskbar in this.taskbars)
                {
                    // update taskbar window
                    UpdateWindow(taskbar.Handle);
                
                    // Set new position for taskbar
                    SetWindowPos(taskbar.Handle, IntPtr.Parse("-1"),
                        taskbar.TaskbarRect.Left, 0,
                        taskbar.TaskbarRect.Width,
                        taskbar.TaskbarRect.Height, 0x0400);

                    // update taskbar window
                    UpdateWindow(taskbar.Handle);
                }
                
                if(this.config.PollingRate > 0)
                    await Task.Delay(this.config.PollingRate);
            } 
            while (!this.cancellationTokenSource.IsCancellationRequested);
        }

        private async Task doWindowPlacementLoopAsync()
        {
            do
            {
                // switcher is not persistent
                this.switcherHwnd = FindWindowA(DESKTOP_SWITCHER_CLASS, DESKTOP_SWITCHER_TITLE);
                if (this.switcherHwnd != IntPtr.Zero) this.placeUnderTaskbar(this.switcherHwnd);

                foreach (var thumbnailHwnd in this.thumbnailHwnds)
                {
                    this.placeUnderTaskbar((IntPtr)thumbnailHwnd);
                }

                this.startHwnd = FindWindowA(START_MENU_CLASS, START_MENU_TITLE);
                if (this.startHwnd != IntPtr.Zero) this.placeStart(this.startHwnd);

                this.systrayOverflowHwnd = FindWindowA(SYSTRAY_OVERFLOW_CLASS, null);
                if (this.systrayOverflowHwnd != IntPtr.Zero) this.placeUnderTaskbar(this.systrayOverflowHwnd);

                if (this.config.PollingRate > 0)
                    await Task.Delay(this.config.PollingRate);
            }
            while (!this.cancellationTokenSource.IsCancellationRequested);
        }
    
        private void placeUnderTaskbar(IntPtr hwnd)
        {
            RECT windowRect;
            RECT clientRect;
            GetWindowRect(hwnd, out windowRect);
            GetClientRect(hwnd, out clientRect);

            if(windowRect.Top != TASKBAR_HEIGHT)
            {
                SetWindowPos(hwnd, IntPtr.Zero, windowRect.Left, TASKBAR_HEIGHT, clientRect.Right, clientRect.Bottom, 0);
            }
        }

        private void placeStart(IntPtr hwnd)
        {
            var x_pos = (this.devmode.dmPelsWidth / 2) - (START_WIDTH / 2);
            SetWindowPos(hwnd, IntPtr.Zero, x_pos, TASKBAR_HEIGHT, START_WIDTH, START_HEIGHT, 0);
        }

        private void restartExplorer()
        {
            try
            {
                var ptr = FindWindowA(TASKBAR_CLASS, null);
                Console.WriteLine("INIT PTR: {0}", ptr.ToInt32());
                PostMessage(ptr, WM_USER + 436, (IntPtr)0, (IntPtr)0);

                do
                {
                    ptr = FindWindowA(TASKBAR_CLASS, null);
                    Console.WriteLine("PTR: {0}", ptr.ToInt32());

                    if (ptr.ToInt32() == 0)
                        break;

                    Thread.Sleep(1000);
                } while (true);
            }
            catch (Exception)
            {
            }
            finally
            {
                string explorer = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");

                var startInfo = new ProcessStartInfo(explorer);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                Environment.Exit(0);
            }
        }
    }
}