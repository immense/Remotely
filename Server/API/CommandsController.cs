using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Attributes;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        public CommandsController(IDataService dataService)
        {
            DataService = dataService;
        }

        private IDataService DataService { get; }

        // GET: api/<controller>
        [HttpGet("{fileExt}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public ActionResult DownloadAll(string fileExt)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var content = "";
            var commandResults = DataService.GetAllCommandResults(orgID);
            switch (fileExt.ToUpper())
            {
                case "JSON":
                    content = System.Text.Json.JsonSerializer.Serialize(commandResults);
                    break;
                case "XML":
                    var serializer = new DataContractSerializer(typeof(CommandResult));
                    using (var ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, commandResults);
                        content = Encoding.UTF8.GetString(ms.ToArray());
                    }
                    break;
                default:
                    throw new Exception("Unknown file type requested.");
            }

            return File(Encoding.UTF8.GetBytes(content), "application/octet-stream", "CommandHistory." + fileExt.ToLower());
        }

        [HttpGet("{fileExt}/{commandID}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public FileResult DownloadResults(string fileExt, string commandID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var content = "";
            var commandResult = DataService.GetCommandResult(commandID, orgID);
            switch (fileExt.ToUpper())
            {
                case "JSON":
                    content = System.Text.Json.JsonSerializer.Serialize(commandResult);
                    break;
                case "XML":
                    var serializer = new DataContractSerializer(typeof(CommandResult));
                    using (var ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, commandResult);
                        content = Encoding.UTF8.GetString(ms.ToArray());
                    }
                    break;
                default:
                    throw new Exception("Unknown file type requested.");
            }

            return File(Encoding.UTF8.GetBytes(content), "application/octet-stream", "CommandResults." + fileExt.ToLower());
        }

        [HttpGet("PSCoreResult/{commandID}/{deviceID}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public PSCoreCommandResult PSCoreResult(string commandID, string deviceID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            return DataService.GetCommandResult(commandID, orgID).PSCoreResults.FirstOrDefault(x => x.DeviceID == deviceID);
        }

        [HttpGet("GenericResult/{commandID}/{deviceID}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public GenericCommandResult GenericResult(string commandID, string deviceID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            return DataService.GetCommandResult(commandID, orgID).CommandResults.FirstOrDefault(x => x.DeviceID == deviceID);
        }

        [HttpPost("{resultType}")]
        public async Task Post(string resultType)
        {
            using var sr = new StreamReader(Request.Body);
            var content = await sr.ReadToEndAsync();
            switch (resultType)
            {
                case "PSCore":
                    {
                        var result = System.Text.Json.JsonSerializer.Deserialize<PSCoreCommandResult>(content);
                        var commandResult = DataService.GetCommandResult(result.CommandResultID);
                        commandResult.PSCoreResults.Add(result);
                        DataService.AddOrUpdateCommandResult(commandResult);
                        break;
                    }
                case "WinPS":
                case "CMD":
                case "Bash":
                    {
                        var result = System.Text.Json.JsonSerializer.Deserialize<GenericCommandResult>(content);
                        var commandResult = DataService.GetCommandResult(result.CommandResultID);
                        commandResult.CommandResults.Add(result);
                        DataService.AddOrUpdateCommandResult(commandResult);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
