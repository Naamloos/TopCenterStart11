namespace TopCenterStart11
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // Thanks stackoverflow for this one https://stackoverflow.com/a/6392077/8468356
            var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            if (exists)
                return;

            var cfg = Config.Load();
            // validating config
            if (cfg == null)
            {
                return;
            }

            var updates = new UpdateChecker(cfg);
            await updates.Check();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new SettingsForm(cfg));
        }
    }
}