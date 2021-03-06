using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.Text.Json;

namespace Remotely.Desktop.Linux.Services
{
    public class ConfigServiceLinux : IConfigService
    {
        private static string ConfigFile => Path.Combine(ConfigFolder, "Config.json");
        private static string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "remotely.json");

        public DesktopAppConfig GetConfig()
        {
            var config = new DesktopAppConfig();

            if (string.IsNullOrWhiteSpace(config.Host) &&
                File.Exists(ConfigFile))
            {
                try
                {
                    config = JsonSerializer.Deserialize<DesktopAppConfig>(File.ReadAllText(ConfigFile));
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }

            return config;
        }

        public void Save(DesktopAppConfig config)
        {
            try
            {
                Directory.CreateDirectory(ConfigFolder);
                File.WriteAllText(ConfigFile, JsonSerializer.Serialize(config));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
