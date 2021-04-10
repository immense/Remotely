using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public class AlertsController : ControllerBase
    {
        public AlertsController(IDataService dataService, IEmailSenderEx emailSender)
        {
            DataService = dataService;
            EmailSender = emailSender;
        }

        private IDataService DataService { get; }
        private IEmailSenderEx EmailSender { get; }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(AlertOptions alertOptions)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            DataService.WriteEvent("Alert created.  Alert Options: " + JsonSerializer.Serialize(alertOptions), orgID);

            if (alertOptions.ShouldAlert)
            {
                try
                {
                    await DataService.AddAlert(alertOptions.AlertDeviceID, orgID, alertOptions.AlertMessage);
                }
                catch (Exception ex)
                {
                    DataService.WriteEvent(ex, orgID);
                }
            }

            if (alertOptions.ShouldEmail)
            {
                try
                {
                    await EmailSender.SendEmailAsync(alertOptions.EmailTo,
                        alertOptions.EmailSubject,
                        alertOptions.EmailBody,
                        orgID);
                }
                catch (Exception ex)
                {
                    DataService.WriteEvent(ex, orgID);
                }

            }

            if (alertOptions.ShouldSendApiRequest)
            {
                try
                {
                    var httpRequest = WebRequest.CreateHttp(alertOptions.ApiRequestUrl);
                    httpRequest.Method = alertOptions.ApiRequestMethod;
                    httpRequest.ContentType = "application/json";
                    foreach (var header in alertOptions.ApiRequestHeaders)
                    {
                        httpRequest.Headers.Add(header.Key, header.Value);
                    }
                    using (var rs = httpRequest.GetRequestStream())
                    using (var sw = new StreamWriter(rs))
                    {
                        sw.Write(alertOptions.ApiRequestBody);
                    }
                    using var response = (HttpWebResponse)httpRequest.GetResponse();
                    DataService.WriteEvent($"Alert API Response Status: {response.StatusCode}.", orgID);
                }
                catch (Exception ex)
                {
                    DataService.WriteEvent(ex, orgID);
                }

            }

            return Ok();
        }

        [HttpDelete("Delete/{alertID}")]
        public async Task<IActionResult> Delete(string alertID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var alert = await DataService.GetAlert(alertID);

            if (alert?.OrganizationID == orgID)
            {
                await DataService.DeleteAlert(alert);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            if (User.Identity.IsAuthenticated)
            {
                await DataService.DeleteAllAlerts(orgID, User.Identity.Name);
            }
            else
            {
                await DataService.DeleteAllAlerts(orgID);
            }

            return Ok();
        }
    }
}
