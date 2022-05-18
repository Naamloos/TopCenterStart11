using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TopCenterStart11
{
    internal class Config
    {
        const string CONFIGFILE = "config.json";

        private static Config instance;

        public int PollingRate { get; set; } = 100;

        public bool FirstRun { get; set; } = true;

        /// <summary>
        /// Do not call the constructor manually. Call Config.Load() instead.
        /// </summary>
        public Config() { }

        public void Save()
        {
            using FileStream fs = getFile(true);
            JsonSerializer.Serialize(fs, this);
            fs.Dispose();
        }

        public static Config Load()
        {
            try
            {
                if (instance == null)
                {
                    using FileStream fs = getFile();
                    instance = JsonSerializer.Deserialize<Config>(fs, new JsonSerializerOptions());
                }

                return instance;
            }
            catch(Exception)
            {
                MessageBox.Show("Your config appears to have been written by JJ Abrams and corrected by Rian Johnson! Please delete it.\n" +
                    "Only touch your config manually if you REALLY know what you are doing.\n\n" +
                    $"Anyway, the file is located here:\n{Path.GetFullPath(CONFIGFILE)}",
                    "Somehow, Palpatine returned...");
                return null;
            }
        }

        private static FileStream getFile(bool clear = false)
        {
            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), CONFIGFILE);
            if (!File.Exists(configPath))
            {
                var fs = File.Create(configPath);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(JsonSerializer.Serialize(new Config()));
                sw.Dispose();
            }

            if (clear)
                File.WriteAllText(configPath, "");
                
            return File.Open(configPath, FileMode.OpenOrCreate);
        }
    }
}
