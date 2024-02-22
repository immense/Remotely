using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;

namespace Remotely.Server.Services.RcImplementations;

public class SessionRecordingSink : ISessionRecordingSink
{
    private readonly ILogger<SessionRecordingSink> _logger;

    public SessionRecordingSink(ILogger<SessionRecordingSink> logger)
    {
        _logger = logger;
    }

    public static string RecordingsDirectory
    {
        get
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                "AppData", 
                "recordings");
        }
    }

    public async Task SinkWebmStream(IAsyncEnumerable<byte[]> webmStream, RemoteControlSession session)
    {
        try
        {
            var targetDir = Path.Combine(RecordingsDirectory, $"{DateTime.Now:yyyy-MM-dd}");
            _ = Directory.CreateDirectory(targetDir);

            var viewerName = !string.IsNullOrWhiteSpace(session.RequesterName) ?
                session.RequesterName : 
                "AnonymousUser";

            var fileName = $"{viewerName}_{DateTime.Now:yyyyMMdd_HHmmssfff}.webm";

            using var fs = new FileStream(Path.Combine(targetDir, fileName), FileMode.Create);

            await foreach (var chunk in webmStream)
            {
                await fs.WriteAsync(chunk);
            }
        }   
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sinking webm stream.");
        }
    }
}
