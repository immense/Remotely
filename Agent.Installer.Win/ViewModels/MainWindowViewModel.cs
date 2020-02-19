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
    public class MainWindowViewModel : ViewModelBase
    {
        private string headerMessage = "Install the Remotely service.";
        private bool isServiceInstalled;
        private bool isServiceMissing = true;
        private string subMessage = "Installing the Remotely service will allow remote access by the above service provider.";
        private string statusMessage;
        private int progress = 50;

        public string HeaderMessage
        {
            get
            {
                return headerMessage;
            }
            set
            {
                headerMessage = value;
                FirePropertyChanged(nameof(HeaderMessage));
            }
        }


        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                FirePropertyChanged(nameof(Progress));
            }
        }

        public bool IsServiceInstalled
        {
            get
            {
                return isServiceInstalled;
            }
            set
            {
                isServiceInstalled = value;
                FirePropertyChanged(nameof(IsServiceInstalled));
            }
        }
        public bool IsServiceMissing
        {
            get
            {
                return isServiceMissing;
            }
            set
            {
                isServiceMissing = value;
                FirePropertyChanged(nameof(IsServiceMissing));
            }
        }
        public string SubMessage
        {
            get
            {
                return subMessage;
            }
            set
            {
                subMessage = value;
                FirePropertyChanged(nameof(SubMessage));
            }
        }

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                FirePropertyChanged(nameof(StatusMessage));
            }
        }
        public async Task Init()
        {
            
        }

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
