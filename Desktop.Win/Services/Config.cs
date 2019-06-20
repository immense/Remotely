using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Win.Services
{
    public class Config
    {
        private Config()
        {

        }

        public string Host { get; set; }
        private static string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely", "Config.json");
        private static string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely");
        public static string GetHostName()
        {
            return GetConfig()?.Host;
        }

        public static void SaveHostName(string hostName)
        {
            try
            {
                var config = GetConfig();
                config.Host = hostName;
                Directory.CreateDirectory(ConfigFolder);
                File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(config));
            }
            catch
            {
            }
        }

        private static Config GetConfig()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                return new Config();
            }

            if (File.Exists(ConfigFile))
            {
                try
                {
                    return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));
                }
                catch
                {
                    return new Config();
                }
            }
            return new Config();
        }
    }
}
