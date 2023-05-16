using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Remotely.Server.Auth;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public class AlertsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IEmailSenderEx _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(
            IDataService dataService, 
            IEmailSenderEx emailSender, 
            IHttpClientFactory httpClientFactory,
            ILogger<AlertsController> logger)
        {
            _dataService = dataService;
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(AlertOptions alertOptions)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            _logger.LogInformation("Alert created.  Alert Options: {options}", JsonSerializer.Serialize(alertOptions));

            if (alertOptions.ShouldAlert)
            {
                try
                {
                    await _dataService.AddAlert(alertOptions.AlertDeviceID, orgID, alertOptions.AlertMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while adding alert.");
                }
            }

            if (alertOptions.ShouldEmail)
            {
                try
                {
                    await _emailSender.SendEmailAsync(alertOptions.EmailTo,
                        alertOptions.EmailSubject,
                        alertOptions.EmailBody,
                        orgID);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while sending email.");
                }

            }

            if (alertOptions.ShouldSendApiRequest)
            {
                try
                {
                    using var httpClient = _httpClientFactory.CreateClient();
                    using var request = new HttpRequestMessage(
                        new HttpMethod(alertOptions.ApiRequestMethod),
                        alertOptions.ApiRequestUrl);

                    request.Content = new StringContent(alertOptions.ApiRequestBody);
                    request.Content.Headers.ContentType.MediaType = "application/json";
                    
                    foreach (var header in alertOptions.ApiRequestHeaders)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }

                    using var response = await httpClient.SendAsync(request);
                    _logger.LogInformation("Alert API Response Status: {responseStatusCode}.", response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while sending alert API request.");
                }

            }

            return Ok();
        }

        [HttpDelete("Delete/{alertID}")]
        public async Task<IActionResult> Delete(string alertID)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var alert = await _dataService.GetAlert(alertID);

            if (alert?.OrganizationID == orgID)
            {
                await _dataService.DeleteAlert(alert);

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
                await _dataService.DeleteAllAlerts(orgID, User.Identity.Name);
            }
            else
            {
                await _dataService.DeleteAllAlerts(orgID);
            }

            return Ok();
        }
    }
}
