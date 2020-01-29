using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Remotely.Shared.Models;
using Remotely.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
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
                    content = System.Text.Json.JsonSerializer.Serialize(commandContexts);
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
                    content = System.Text.Json.JsonSerializer.Serialize(commandContext);
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
        public async Task Post(string resultType)
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var content = await sr.ReadToEndAsync();
                switch (resultType)
                {
                    case "PSCore":
                        {
                            var result = System.Text.Json.JsonSerializer.Deserialize<PSCoreCommandResult>(content);
                            var commandContext = DataService.GetCommandContext(result.CommandContextID);
                            commandContext.PSCoreResults.Add(result);
                            DataService.AddOrUpdateCommandContext(commandContext);
                            break;
                        }
                    case "WinPS":
                    case "CMD":
                    case "Bash":
                        {
                            var result = System.Text.Json.JsonSerializer.Deserialize<GenericCommandResult>(content);
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
}
