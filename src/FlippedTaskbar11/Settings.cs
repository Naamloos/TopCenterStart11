using FlippedTaskbar11.TaskbarLogics;

namespace FlippedTaskbar11
{
    public partial class SettingsForm : Form
    {
        private TaskbarManager taskbar;

        public SettingsForm()
        {
            InitializeComponent();

            taskbar = new TaskbarManager();

            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripButton("Show Settings", null, onTrayShow, "Show Settings"),
                new ToolStripButton("Exit", null, onTrayExit, "Exit")
            });

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
            taskbar.StopTaskbarLoop();
            this.Dispose();
            Application.Exit();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }
    }
}