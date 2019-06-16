using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Remotely.Shared.Models;
using Remotely.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    public class CommandsController : Controller
    {
        public CommandsController(DataService dataService)
        {
            this.DataService = dataService;
        }

        private DataService DataService { get; }

        // GET: api/<controller>
        [HttpGet("{fileExt}")]
        [Authorize]
        public ActionResult DownloadAll(string fileExt)
        {
            if (!DataService.GetUserByName(User.Identity.Name).IsAdministrator)
            {
                return Unauthorized();
            }

            var content = "";
            var commandContexts = DataService.GetAllCommandContexts(User.Identity.Name);
            switch (fileExt.ToUpper())
            {
                case "JSON":
                    content = JsonConvert.SerializeObject(commandContexts);
                    break;
                case "XML":
                    var serializer = new DataContractSerializer(typeof(CommandContext));
                    using (var ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, commandContexts);
                        content = Encoding.UTF8.GetString(ms.ToArray());
                    }
                    break;
                default:
                    throw new Exception("Unknown file type requested.");
            }

            return File(Encoding.UTF8.GetBytes(content), "application/octet-stream", "CommandHistory." + fileExt.ToLower());
        }

        [HttpGet("{fileExt}/{commandID}")]
        [Authorize]
        public FileResult DownloadResults(string fileExt, string commandID)
        {
            var content = "";
            var commandContext = DataService.GetCommandContext(commandID, User.Identity.Name);
            switch (fileExt.ToUpper())
            {
                case "JSON":
                    content = JsonConvert.SerializeObject(commandContext);
                    break;
                case "XML":
                    var serializer = new DataContractSerializer(typeof(CommandContext));
                    using (var ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, commandContext);
                        content = Encoding.UTF8.GetString(ms.ToArray());
                    }
                    break;
                default:
                    throw new Exception("Unknown file type requested.");
            }

            return File(Encoding.UTF8.GetBytes(content), "application/octet-stream", "CommandResults." + fileExt.ToLower());
        }

        [HttpGet("PSCoreResult/{commandID}/{deviceID}")]
        [Authorize]
        public PSCoreCommandResult PSCoreResult(string commandID, string deviceID)
        {
            return DataService.GetCommandContext(commandID, User.Identity.Name).PSCoreResults.FirstOrDefault(x => x.DeviceID == deviceID);
        }

        [HttpGet("GenericResult/{commandID}/{deviceID}")]
        [Authorize]
        public GenericCommandResult GenericResult(string commandID, string deviceID)
        {
            return DataService.GetCommandContext(commandID, User.Identity.Name).CommandResults.FirstOrDefault(x => x.DeviceID == deviceID);
        }

        [HttpPost("{resultType}")]
        public void Post(string resultType)
        {
            var content = new StreamReader(Request.Body).ReadToEnd();
            switch (resultType)
            {
                case "PSCore":
                    {
                        var result = JsonConvert.DeserializeObject<PSCoreCommandResult>(content);
                        var commandContext = DataService.GetCommandContext(result.CommandContextID);
                        commandContext.PSCoreResults.Add(result);
                        DataService.AddOrUpdateCommandContext(commandContext);
                        break;
                    }
                case "WinPS":
                case "CMD":
                case "Bash":
                    {
                        var result = JsonConvert.DeserializeObject<GenericCommandResult>(content);
                        var commandContext = DataService.GetCommandContext(result.CommandContextID);
                        commandContext.CommandResults.Add(result);
                        DataService.AddOrUpdateCommandContext(commandContext);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
