using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Do not call the constructor manually. Call Config.Load() instead.
        /// </summary>
        public Config() { }

        public void Save()
        {
            using FileStream fs = getFile();
            fs.Position = 0;
            JsonSerializer.Serialize(fs, this);
        }

        public static Config Load()
        {
            if (instance == null)
            {
                using FileStream fs = getFile();
                instance = JsonSerializer.Deserialize<Config>(fs, new JsonSerializerOptions());
            }

            return instance;
        }

        private static FileStream getFile()
        {
            if (!File.Exists("config.json"))
            {
                var fs = File.Create("config.json");
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(JsonSerializer.Serialize(new Config()));
                sw.Dispose();
            }
                
            return File.Open("config.json", FileMode.OpenOrCreate);
        }
    }
}
