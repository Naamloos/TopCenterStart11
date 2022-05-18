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

            if (config.FirstRun)
                doSetupStuff();

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
                var path = Assembly.GetEntryAssembly().Location;
                if (path.EndsWith(".dll"))
                    path = path.Replace(".dll", ".exe");
                key.SetValue(REGISTRY_ITEM, path);
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

            MessageBox.Show("Your configuration has been saved.");
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

        private void doSetupStuff()
        {
            var dialog = MessageBox.Show("Welcome!\nThis is your first time running TopCenterStart11." +
                "\n\nWould you like to Enable Autostart?\n" +
                "You can always change this via the settings (tray icon will display in a bit).", 
                "Welcome to TopCenterStart11!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(dialog == DialogResult.Yes)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_AUTOSTART_PATH, true);

                if (getAutoStartEnabled())
                {
                    key.DeleteValue(REGISTRY_ITEM, false);
                }

                var path = Assembly.GetEntryAssembly().Location;
                if (path.EndsWith(".dll"))
                    path = path.Replace(".dll", ".exe");
                key.SetValue(REGISTRY_ITEM, path);
            }

            config.FirstRun = false;
            config.Save();
        }
    }
}