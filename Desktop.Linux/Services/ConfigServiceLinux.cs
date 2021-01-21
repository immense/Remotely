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
        private static string _configFile => Path.Combine(_configFolder, "Config.json");
        private static string _configFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "remotely.json");

        public DesktopAppConfig GetConfig()
        {
            var config = new DesktopAppConfig();

            if (string.IsNullOrWhiteSpace(config.Host) &&
                File.Exists(_configFile))
            {
                try
                {
                    config = JsonSerializer.Deserialize<DesktopAppConfig>(File.ReadAllText(_configFile));
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
                Directory.CreateDirectory(_configFolder);
                File.WriteAllText(_configFile, JsonSerializer.Serialize(config));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
