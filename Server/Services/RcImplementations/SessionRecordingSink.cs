using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
            var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recordings");
            if (Directory.Exists("/remotely-data"))
            {
                logsDir = "/remotely-data/recordings";
            }
            return logsDir;
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
