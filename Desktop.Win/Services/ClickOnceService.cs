using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Remotely.Desktop.Win.Services
{
    public interface IClickOnceService
    {
        string GetActivationUri();
    }
    public class ClickOnceService : IClickOnceService
    {
        private static string _activationUri;
        public string GetActivationUri()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_activationUri))
                {
                    return _activationUri;
                }
                var appRoot = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
                if (!Directory.Exists(Path.Combine(appRoot.FullName, "manifests")))
                {
                    Logger.Write($"Manifests folder not found in root folder: {appRoot}", EventType.Warning);
                    return _activationUri;
                }

                var manifestFiles = appRoot.GetFiles("manifests\\*tion_*.manifest", SearchOption.AllDirectories);
                var manifestFile = manifestFiles.FirstOrDefault();
                if (manifestFile is null)
                {
                    Logger.Write($"Manifest file not found.", EventType.Warning);
                    return _activationUri;
                }

                var manifest = new XmlDocument();
                manifest.Load(manifestFile.FullName);
                var node = manifest.GetElementsByTagName("deploymentProvider")[0];
                _activationUri = node.Attributes["codebase"].Value;

                Logger.Write($"Found ActivationUri: {_activationUri}");
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            return _activationUri;
        }
    }
}
