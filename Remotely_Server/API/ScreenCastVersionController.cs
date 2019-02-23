using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

namespace Remotely_Server.API
{
    [Route("api/[controller]")]
    public class ScreenCastVersionController : Controller
    {
        public ScreenCastVersionController(IHostingEnvironment hostingEnv)
        {
            this.HostingEnv = hostingEnv;
        }

        public IHostingEnvironment HostingEnv { get; }

        // GET: api/<controller>
        [HttpGet("{platform}")]
        public string Get(string platform)
        {
            string ext = "";

            switch (platform)
            {
                case "Windows":
                    ext = "exe";
                    break;
                case "Linux":
                    ext = "appimage";
                    break;
                default:
                    break;
            }
            var filePath = Path.Combine(HostingEnv.WebRootPath, "Downloads", $"Remotely_ScreenCast.{ext}");
            if (!System.IO.File.Exists(filePath))
            {
                return "0.0.0.0";
            }
            var version = FileVersionInfo.GetVersionInfo(filePath);
            return version.FileVersion.ToString();
        }
    }
}
