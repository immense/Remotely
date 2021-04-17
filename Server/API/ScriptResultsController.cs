using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScriptResultsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IEmailSenderEx _emailSender;

        public ScriptResultsController(IDataService dataService, IEmailSenderEx emailSenderEx)
        {
            _dataService = dataService;
            _emailSender = emailSenderEx;
        }


        // GET: api/<controller>
        [HttpGet]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public ActionResult DownloadAll()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var commandResults = _dataService.GetAllCommandResults(orgID);
            var content = System.Text.Json.JsonSerializer.Serialize(commandResults);
            return File(Encoding.UTF8.GetBytes(content), "application/octet-stream", "ScriptHistory.json");       
        }

        [HttpGet("{scriptId}")]
        [ServiceFilter(typeof(ApiAuthorizationFilter))]
        public FileResult DownloadResults(string scriptId)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var commandResult = _dataService.GetScriptResult(scriptId, orgID);
            var content = System.Text.Json.JsonSerializer.Serialize(commandResult);
            return File(Encoding.UTF8.GetBytes(content), "application/octet-stream", "ScriptResults.json");
        }


        [HttpPost]
        [ServiceFilter(typeof(ExpiringTokenFilter))]
        public async Task<ScriptResult> Post([FromBody] ScriptResult result)
        {
            _dataService.AddOrUpdateScriptResult(result);

            if (result.HadErrors && result.SavedScriptId.HasValue)
            {
                var savedScript = await _dataService.GetSavedScript(result.SavedScriptId.Value);
                if (savedScript.GenerateAlertOnError)
                {
                    await _dataService.AddAlert(result.DeviceID,
                        result.OrganizationID,
                        $"Alert triggered while running script {savedScript.Name}.",
                        string.Join("\n", result.ErrorOutput));
                }

                if (savedScript.SendEmailOnError)
                {
                    var device = _dataService.GetDevice(result.DeviceID);

                    await _emailSender.SendEmailAsync(savedScript.SendErrorEmailTo,
                        "Script Run Alert",
                        $"An alert was triggered while running script {savedScript.Name} on device {device.DeviceName}. <br /><br />" +
                            $"Error Output: <br /><br /> " +
                            $"{string.Join("<br /><br />", result.ErrorOutput)}");
                }
            }

            if (result.ScriptRunId.HasValue)
            {
                await _dataService.AddScriptResultToScriptRun(result.ID, result.ScriptRunId.Value);
            }

            return result;
        }
    }
}
