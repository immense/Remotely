using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_Desktop.Unix.Services
{
    public class Config
    {
        private static string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely");
        private static string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely", "Config.json");

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
                    return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));
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
                File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(this));
            }
            catch
            {
            }
        }
        public string Host { get; set; }
    }
}
