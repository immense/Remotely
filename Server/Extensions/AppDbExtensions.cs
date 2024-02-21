using Remotely.Server.Data;
using Remotely.Server.Models;
using System.Text.Json;

namespace Remotely.Server.Extensions;

public static class AppDbExtensions
{
    public static async Task<SettingsModel> GetAppSettings(this AppDb dbContext)
    {
        var record = await dbContext.KeyValueRecords.FindAsync(SettingsModel.DbKey);
        if (record is null)
        {
            record = new()
            {
                Key = SettingsModel.DbKey,
            };
            await dbContext.KeyValueRecords.AddAsync(record);
            await dbContext.SaveChangesAsync();
        }

        if (string.IsNullOrWhiteSpace(record.Value))
        {
            var settings = new SettingsModel();
            record.Value = JsonSerializer.Serialize(settings);
            await dbContext.SaveChangesAsync();
        }

        return JsonSerializer.Deserialize<SettingsModel>(record.Value) ?? new();
    }
}
