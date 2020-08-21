using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.Text.Json;

namespace Remotely.Desktop.Win.Services
{
    public class Config
    {
        public string Host { get; set; } = "";
        private static string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely", "Config.json");
        private static string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remotely");
        public static Config GetConfig()
        {

            if (File.Exists(ConfigFile))
            {
                try
                {
                    return JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigFile));
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
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
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
