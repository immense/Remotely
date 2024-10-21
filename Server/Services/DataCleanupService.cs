using Remotely.Shared.Services;

namespace Remotely.Server.Services;

public class DataCleanupService : BackgroundService, IDisposable
{
    private readonly ILogger<DataCleanupService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISystemTime _systemTime;

    public DataCleanupService(
        IServiceScopeFactory scopeFactory,
        ISystemTime systemTime,
        ILogger<DataCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _systemTime = systemTime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        await PerformCleanup();

        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _ = await timer.WaitForNextTickAsync(stoppingToken);
                await PerformCleanup();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Application is shutting down.  Stopping data cleanup service.");
            }

        }
    }

    private async Task PerformCleanup()
    {
        try
        {
            await RemoveExpiredDbRecords();
            await RemoveExpiredRecordings();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data cleanup.");
        }
    }

    private async Task RemoveExpiredDbRecords()
    {
        using var scope = _scopeFactory.CreateScope();
        var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
        await dataService.CleanupOldRecords();
    }

    private async Task RemoveExpiredRecordings()
    {
        using var scope = _scopeFactory.CreateScope();
        var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
        var settings = await dataService.GetSettings();

        if (!Directory.Exists(SessionRecordingSink.RecordingsDirectory))
        {
            return;
        }

        var expirationDate = _systemTime.Now.UtcDateTime - TimeSpan.FromDays(settings.DataRetentionInDays);

        var files = Directory
            .GetFiles(
                SessionRecordingSink.RecordingsDirectory,
                "*.webm",
                SearchOption.AllDirectories)
            .Select(x => new FileInfo(x))
            .Where(x => x.CreationTimeUtc < expirationDate)
            .ToList();

        foreach (var file in files)
        {
            try
            {
                file.Delete();
                _logger.LogInformation("Expired recording deleted: {file}", file.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting expired recording: {file}", file);
            }
        }
    }
}
