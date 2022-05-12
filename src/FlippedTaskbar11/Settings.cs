using Microsoft.Win32;
using System.Reflection;
using TopCenterStart11.TaskbarLogics;

namespace TopCenterStart11
{
    public partial class SettingsForm : Form
    {
        const string ENABLE_AUTOSTART = "Enable Autostart";
        const string DISABLE_AUTOSTART = "Disable Autostart";
        const string REGISTRY_ITEM = "TopCenterStart11";
        const string REGISTRY_AUTOSTART_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private Config config;
        private TaskbarManager taskbar;

        public SettingsForm()
        {
            InitializeComponent();

            config = Config.Load();

            setAppropriateAutoStartText();
            taskbar = new TaskbarManager(config);

            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripLabel("Welcome to TopCenterStart11! <3"),
                new ToolStripButton("Show Settings", null, onTrayShow, "Show Settings"),
                new ToolStripButton("Exit", null, onTrayExit, "Exit")
            });

            pollingRate.Value = config.PollingRate;

            taskbar.StartTaskbarLoop();
        }

        private void onTrayIconClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.BringToFront();
        }

        private void onTrayShow(object? sender, EventArgs eventargs)
        {
            onTrayIconClick(null, null);
        }

        private void onTrayExit(object? sender, EventArgs eventargs)
        {
            if(taskbar != null)
                taskbar.StopTaskbarLoop();

            this.Dispose();
            Application.Exit();
        }

        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }

        private void onAutoStartClick(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_AUTOSTART_PATH, true);

            if (!getAutoStartEnabled())
            {
                key.SetValue(REGISTRY_ITEM, Assembly.GetEntryAssembly().Location);
            }
            else
            {
                key.DeleteValue(REGISTRY_ITEM, false);
            }

            setAppropriateAutoStartText();
        }

        private void onSaveClick(object sender, EventArgs e)
        {
            // Copy config values from the UI to the config and save
            config.PollingRate = (int)pollingRate.Value;
            config.Save();
        }

        private void setAppropriateAutoStartText()
        {
            autoStart.Text = getAutoStartEnabled() ? DISABLE_AUTOSTART : ENABLE_AUTOSTART;
        }

        private bool getAutoStartEnabled()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);

            return key.GetValueNames().Contains(REGISTRY_ITEM);
        }
    }
}