using Immense.RemoteControl.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Remotely.Server.Auth;
using Remotely.Server.Extensions;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.API;

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
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        _logger.LogInformation("Alert created.  Alert Options: {options}", JsonSerializer.Serialize(alertOptions));

        if (alertOptions.ShouldAlert)
        {
            try
            {
                await _dataService.AddAlert(alertOptions.AlertDeviceID, orgId, alertOptions.AlertMessage);
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
                if (string.IsNullOrWhiteSpace(alertOptions.EmailTo))
                {
                    return BadRequest("Email address is required to send email.");
                }
                if (string.IsNullOrWhiteSpace(alertOptions.EmailSubject))
                {
                    return BadRequest("Email subject is required to send email.");
                }
                if (string.IsNullOrWhiteSpace(alertOptions.EmailBody))
                {
                    return BadRequest("Email body is required to send email.");
                }
                await _emailSender.SendEmailAsync(
                    alertOptions.EmailTo,
                    alertOptions.EmailSubject,
                    alertOptions.EmailBody,
                    orgId.ToString());
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
                if (string.IsNullOrWhiteSpace(alertOptions.ApiRequestUrl))
                {
                    return BadRequest("API request URL is required to send API request.");
                }
                if (string.IsNullOrWhiteSpace(alertOptions.ApiRequestMethod))
                {
                    return BadRequest("API request method is required to send API request.");
                }
                if (string.IsNullOrWhiteSpace(alertOptions.ApiRequestBody))
                {
                    return BadRequest("API request body is required to send API request.");
                }

                using var httpClient = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(
                    new HttpMethod(alertOptions.ApiRequestMethod),
                    alertOptions.ApiRequestUrl);

                request.Content = new StringContent(alertOptions.ApiRequestBody);
                request.Content.Headers.ContentType = new("application/json");
                
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
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        var alertResult = await _dataService.GetAlert(alertID);
        _logger.LogResult(alertResult);
        if (!alertResult.IsSuccess)
        {
            return BadRequest(alertResult.Reason);
        }

        if (alertResult.Value.OrganizationID != orgId)
        {
            return Unauthorized();
        }

        await _dataService.DeleteAlert(alertResult.Value);
        return Ok();
    }

    [HttpDelete("DeleteAll")]
    public async Task<IActionResult> DeleteAll()
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            await _dataService.DeleteAllAlerts(orgId.ToString(), User?.Identity?.Name);
        }
        else
        {
            await _dataService.DeleteAllAlerts(orgId.ToString());
        }

        return Ok();
    }
}
