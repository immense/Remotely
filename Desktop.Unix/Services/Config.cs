using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Desktop.Unix.Services
{
    public class Config
    {
        public string Host { get; set; } = "";
        private static string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely", "Config.json");
        private static string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely");
        public static Config GetConfig()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                return new Config();
            }

            if (File.Exists(ConfigFile))
            {
                try
                {
                    return JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigFile));
                }
                catch
                {
                    return new Config();
                }
            }
            return new Config();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(ConfigFolder);
                File.WriteAllText(ConfigFile, JsonSerializer.Serialize(this));
            }
            catch
            {
            }
        }
    }
}
