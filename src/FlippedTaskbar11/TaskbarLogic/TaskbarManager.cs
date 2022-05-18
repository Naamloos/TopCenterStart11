using TopCenterStart11.TaskbarLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TopCenterStart11.TaskbarLogics
{
    internal class TaskbarManager
    {
        public const string START_MENU_CLASS = "Windows.UI.Core.CoreWindow";
        public const string START_MENU_TITLE = "Start";

        public const string THUMBNAIL_CLASS = "TaskListThumbnailWnd";

        public const string DESKTOP_SWITCHER_CLASS = "XamlExplorerHostIslandWindow";
        public const string DESKTOP_SWITCHER_TITLE = "";

        public const string TASKBAR_CLASS = "Shell_TrayWnd";

        public const string POPUP_TITLE = "PopupHost";
        public const string POPUP_CLASS = "Xaml_WindowedPopupClass";

        public const int START_WIDTH = 666;
        public const int START_HEIGHT = 750;

        public const int TASKBAR_HEIGHT = 48;

        private Config config;

        private IntPtr startHwnd;
        private IntPtr thumbnailHwnd;
        private IntPtr taskbarHwnd;
        private IntPtr switcherHwnd;

        private WIN32.DEVMODE devmode;
        private WIN32.RECT workingArea;
        private WIN32.RECT originalWorkingArea;

        private WIN32.RECT taskbarWindowRect;
        private WIN32.RECT taskbarClientRect;

        private CancellationTokenSource cancellationTokenSource;
        private bool valid = true;
        private bool stopped = false;

        public TaskbarManager(Config config)
        {
            this.config = config;
            cancellationTokenSource = new CancellationTokenSource();

            // pre-fetch windows on launch.
            startHwnd = WIN32.FindWindowA(START_MENU_CLASS, START_MENU_TITLE);
            thumbnailHwnd = WIN32.FindWindowA(THUMBNAIL_CLASS, null);
            taskbarHwnd = WIN32.FindWindowA(TASKBAR_CLASS, null);
            switcherHwnd = WIN32.FindWindowA(DESKTOP_SWITCHER_CLASS, DESKTOP_SWITCHER_TITLE);
            // some of these are not persistent,
            // so they require polling (unfortunately)
            // these are stored class-wide anyway to prevent
            // constant re-allocation whatever

            // -1 is for ENUM_CURRENT_SETTINGS, -2 is for ENUM_REGISTRY_SETTINGS
            WIN32.EnumDisplaySettings(null, -1, ref this.devmode);
            // obviously we want current settings.

            workingArea = new WIN32.RECT();

            unsafe
            {
                var working = new WIN32.RECT();
                var pointer = &working;
                WIN32.SystemParametersInfo(WIN32.SPI_GETWORKAREA, 0, (IntPtr)pointer, 0);
                workingArea = working;
            }

            // new™️ and improved™️ working area
            workingArea.Top += 48;
            workingArea.Bottom += 48;

            WIN32.GetWindowRect(taskbarHwnd, out taskbarWindowRect);
            WIN32.GetClientRect(taskbarHwnd, out taskbarClientRect);
        }

        public void StartTaskbarLoop()
        {
            if (!valid)
                throw new Exception("This taskbar manager has already started once. Please onstruct a new one to restart it.");

            // set new working area
            unsafe
            {
                var working = workingArea;
                var pointer = &working;
                WIN32.SystemParametersInfo(WIN32.SPI_SETWORKAREA, 0, (IntPtr)pointer, 0x02);
                workingArea = working;
            }

            _ = Task.Run(doTaskbarLoopAsync, cancellationTokenSource.Token);
            _ = Task.Run(doWindowPlacementLoopAsync, cancellationTokenSource.Token);

            valid = false;
        }

        public void StopTaskbarLoop()
        {
            if (!stopped && !valid)
            {
                try
                {
                    // refetch working area

                    unsafe
                    {
                        var working = new WIN32.RECT();
                        var pointer = &working;
                        WIN32.SystemParametersInfo(WIN32.SPI_GETWORKAREA, 0, (IntPtr)pointer, 0);
                        workingArea = working;
                    }

                    // old™️ and unimproved™️ working area
                    workingArea.Top -= 48;
                    workingArea.Bottom -= 48;

                    unsafe
                    {
                        var working = workingArea;
                        var pointer = &working;
                        WIN32.SystemParametersInfo(WIN32.SPI_SETWORKAREA, 0, (IntPtr)pointer, 0x02);
                        workingArea = working;
                    }
                    WIN32.UpdateWindow(taskbarHwnd);

                    this.cancellationTokenSource.Cancel();
                }
                finally
                {
                    restartExplorer();
                }
                stopped = true;
            }
        }

        private async Task doTaskbarLoopAsync()
        {
            do
            {
                // update taskbar window
                WIN32.UpdateWindow(taskbarHwnd);
                // Set new position for taskbar
                WIN32.SetWindowPos(taskbarHwnd, IntPtr.Parse("-1"), taskbarWindowRect.Left, 0,
                    taskbarWindowRect.Right, taskbarClientRect.Bottom, 0x0400);

                // Update taskbar
                WIN32.UpdateWindow(taskbarHwnd);

                if(config.PollingRate > 0)
                    await Task.Delay(config.PollingRate);
            } 
            while (!cancellationTokenSource.IsCancellationRequested);
        }

        private async Task doWindowPlacementLoopAsync()
        {
            do
            {
                // switcher is not persistent
                switcherHwnd = WIN32.FindWindowA(DESKTOP_SWITCHER_CLASS, DESKTOP_SWITCHER_TITLE);
                if (switcherHwnd != IntPtr.Zero)
                    placeUnderTaskbar(switcherHwnd);

                placeUnderTaskbar(thumbnailHwnd);
                placeStart(startHwnd);

                if (config.PollingRate > 0)
                    await Task.Delay(config.PollingRate);
            }
            while (!cancellationTokenSource.IsCancellationRequested);
        }

        private void placeUnderTaskbar(IntPtr hwnd)
        {
            WIN32.RECT windowRect;
            WIN32.RECT clientRect;
            WIN32.GetWindowRect(hwnd, out windowRect);
            WIN32.GetClientRect(hwnd, out clientRect);

            if(windowRect.Top != TASKBAR_HEIGHT)
            {
                WIN32.SetWindowPos(hwnd, IntPtr.Zero, windowRect.Left, TASKBAR_HEIGHT, clientRect.Right, clientRect.Bottom, 0);
            }
        }

        private void placeStart(IntPtr hwnd)
        {
            var x_pos = (devmode.dmPelsWidth / 2) - (START_WIDTH / 2);
            WIN32.SetWindowPos(hwnd, IntPtr.Zero, x_pos, TASKBAR_HEIGHT, START_WIDTH, START_HEIGHT, 0);
        }

        private void restartExplorer()
        {
            try
            {
                var ptr = WIN32.FindWindowA(TASKBAR_CLASS, null);
                Console.WriteLine("INIT PTR: {0}", ptr.ToInt32());
                WIN32.PostMessage(ptr, WIN32.WM_USER + 436, (IntPtr)0, (IntPtr)0);

                do
                {
                    ptr = WIN32.FindWindowA(TASKBAR_CLASS, null);
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
