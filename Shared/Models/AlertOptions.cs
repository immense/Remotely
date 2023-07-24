using System.Collections.Generic;

namespace Remotely.Shared.Models;

public class AlertOptions
{
    public string AlertDeviceID { get; set; } = string.Empty;
    public string AlertMessage { get; set; } = string.Empty;
    public string ApiRequestBody { get; set; } = string.Empty;
    public Dictionary<string, string> ApiRequestHeaders { get; set; } = new();
    public string ApiRequestMethod { get; set; } = string.Empty;
    public string ApiRequestUrl { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailTo { get; set; } = string.Empty;
    public bool ShouldAlert { get; set; }
    public bool ShouldEmail { get; set; }
    public bool ShouldSendApiRequest { get; set; }
}
