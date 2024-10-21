using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Auth;
using Remotely.Server.Services;

namespace Remotely.Server.API;

[Route("api/[controller]")]
[ApiController]
public class ServerLogsController : ControllerBase
{
    private readonly ILogsManager _logsManager;
    private readonly ILogger<ServerLogsController> _logger;

    public ServerLogsController(
        ILogsManager logsManager,
        ILogger<ServerLogsController> logger)
    {
        _logsManager = logsManager;
        _logger = logger;
    }

    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    [HttpGet("Download")]
    public async Task<IActionResult> Download()
    {
        _logger.LogInformation(
            "Downloading server logs. Remote IP: {ip}",
            HttpContext.Connection.RemoteIpAddress);

        var zipFile = await _logsManager.ZipAllLogs();

        Response.OnCompleted(() =>
        {
            if (zipFile.Directory is null)
            {
                return Task.CompletedTask;
            }

            zipFile.Directory.Delete(true);
            return Task.CompletedTask;
        });

        return File(zipFile.OpenRead(), "application/octet-stream", zipFile.Name);
    }
}
