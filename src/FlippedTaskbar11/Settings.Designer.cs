namespace TopCenterStart11
{
    partial class SettingsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.autoStart = new System.Windows.Forms.Button();
            this.pollingRate = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.saveConfig = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pollingRate)).BeginInit();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.BalloonTipText = "Taskbar mover is Enabled";
            this.trayIcon.BalloonTipTitle = "Running...";
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "FlippedTaskbar11";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.onTrayIconClick);
            // 
            // autoStart
            // 
            this.autoStart.Location = new System.Drawing.Point(12, 12);
            this.autoStart.Name = "autoStart";
            this.autoStart.Size = new System.Drawing.Size(196, 23);
            this.autoStart.TabIndex = 0;
            this.autoStart.Text = "Enable Autostart";
            this.autoStart.UseVisualStyleBackColor = true;
            this.autoStart.Click += new System.EventHandler(this.onAutoStartClick);
            // 
            // pollingRate
            // 
            this.pollingRate.Location = new System.Drawing.Point(12, 56);
            this.pollingRate.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.pollingRate.Name = "pollingRate";
            this.pollingRate.Size = new System.Drawing.Size(196, 23);
            this.pollingRate.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Polling rate";
            // 
            // saveConfig
            // 
            this.saveConfig.Location = new System.Drawing.Point(12, 85);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(196, 23);
            this.saveConfig.TabIndex = 3;
            this.saveConfig.Text = "Save Config";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.onSaveClick);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 116);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pollingRate);
            this.Controls.Add(this.autoStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TopCenterStart11";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pollingRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NotifyIcon trayIcon;
        private Button autoStart;
        private NumericUpDown pollingRate;
        private Label label1;
        private Button saveConfig;
    }
}