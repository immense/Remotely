using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Services;

namespace Remotely.Server.API;

[Route("api/custom-binaries")]
[ApiController]
public class CustomBinariesController(
    IDataService _dataService,
    IWebHostEnvironment _hostingEnvironment,
    IEmbeddedServerDataProvider _embeddedData) : ControllerBase
{
    [HttpGet("win-x86/desktop/{organizationId}")]
    public async Task<IActionResult> GetWinX86Desktop(string organizationId)
    {
        var embeddedData = await GetEmbeddedData(organizationId);
        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "AppData", "Win-x86", "Remotely_Desktop.exe");
        var fileName = _embeddedData.GetEncodedFileName(filePath, embeddedData);
        var rs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(rs, "application/octet-stream", fileName);
    }

    [HttpGet("win-x64/desktop/{organizationId}")]
    public async Task<IActionResult> GetWinX64Desktop(string organizationId)
    {
        var embeddedData = await GetEmbeddedData(organizationId);
        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "AppData", "Win-x64", "Remotely_Desktop.exe");
        var fileName = _embeddedData.GetEncodedFileName(filePath, embeddedData);
        var rs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(rs, "application/octet-stream", fileName);
    }

    private async Task<EmbeddedServerData> GetEmbeddedData(string? organizationId)
    {
        var defaultOrg = await _dataService.GetDefaultOrganization();

        // The default org will be used if unspecified, so might as well save the
        // space in the file name.
        if (defaultOrg.IsSuccess &&
            defaultOrg.Value.ID.Equals(organizationId, StringComparison.OrdinalIgnoreCase))
        {
            organizationId = null;
        }

        var settings = await _dataService.GetSettings();
        var effectiveScheme = settings.ForceClientHttps ? "https" : Request.Scheme;
        var serverUrl = $"{effectiveScheme}://{Request.Host}";
        return new EmbeddedServerData(new Uri(serverUrl), organizationId);
    }
}
