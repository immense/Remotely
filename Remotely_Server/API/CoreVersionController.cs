using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Remotely_Server.API
{
    [Route("api/[controller]")]
    public class CoreVersionController : Controller
    {
        public CoreVersionController(IHostingEnvironment hostingEnv)
        {
            this.HostingEnv = hostingEnv;
        }

        public IHostingEnvironment HostingEnv { get; }

        // GET: api/<controller>
        [HttpGet("{platform}")]
        public string Get(string platform)
        {
            string fileName = "";
            switch (platform)
            {
                case "Windows":
                    fileName = "Remotely-Win10-x64.zip";
                    break;
                case "Linux":
                    fileName = "Remotely-Linux.zip";
                    break;
                default:
                    return "";
            }
            var filePath = Path.Combine(HostingEnv.WebRootPath, "Downloads", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return "0.0.0.0";
            }
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var zipArchive = new ZipArchive(fs);
                var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                zipArchive.GetEntry("Remotely_Agent.dll").ExtractToFile(tempFile, true);
                var version = FileVersionInfo.GetVersionInfo(tempFile);
                return version.FileVersion.ToString();
            }
        }
    }
}
