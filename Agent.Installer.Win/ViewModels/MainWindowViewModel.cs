using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Installer.Win.ViewModels
{
    public class MainWindowViewModel
    {
        public InstallerSettings ReadInstallerSettings()
        {
            try
            {
                var fileBytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
                using (var readStream = new MemoryStream(fileBytes))
                using (var br = new BinaryReader(readStream))
                {
                    readStream.Seek(-4, SeekOrigin.End);
                    var payloadSize = br.ReadInt32();
                    readStream.Seek(-4 - payloadSize, SeekOrigin.End);
                    var payloadBytes = br.ReadBytes(payloadSize);
                    using (var writeStream = new MemoryStream(payloadBytes))
                    {
                        writeStream.Seek(0, SeekOrigin.Begin);
                        var serializer = new DataContractJsonSerializer(typeof(InstallerSettings));
                        var installerSettings = (InstallerSettings)serializer.ReadObject(writeStream);
                        return installerSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
