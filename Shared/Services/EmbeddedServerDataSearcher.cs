#nullable enable

using Immense.RemoteControl.Shared;
using MessagePack;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Entities;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Shared.Services;

public interface IEmbeddedServerDataSearcher
{
    Task<Result<AppendableStream>> GetAppendedStream(string filePath, EmbeddedServerData serverData);
    Task<Result<EmbeddedServerData>> TryGetEmbeddedData(string filePath);
}

public class EmbeddedServerDataSearcher() : IEmbeddedServerDataSearcher
{
    public static EmbeddedServerDataSearcher Instance { get; } = new();

    public async Task<Result<EmbeddedServerData>> TryGetEmbeddedData(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return Result.Fail<EmbeddedServerData>($"File path does not exist: {filePath}");
            }

            using var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var br = new BinaryReader(fs);
            using var sr = new StreamReader(fs);

            fs.Seek(-4, SeekOrigin.End);
            var dataSize = br.ReadInt32();
            fs.Seek(-dataSize - 4, SeekOrigin.End);

            var buffer = new byte[dataSize];
            await fs.ReadExactlyAsync(buffer);
            var json = Encoding.UTF8.GetString(buffer);

            var embeddedData = JsonSerializer.Deserialize<EmbeddedServerData>(json);

            if (embeddedData is null)
            {
                return Result.Fail<EmbeddedServerData>("Embedded data is empty.");
            }

            return Result.Ok(embeddedData);
        }
        catch (Exception ex)
        {
            return Result.Fail<EmbeddedServerData>(ex);
        }
    }

    public Task<Result<AppendableStream>> GetAppendedStream(string filePath, EmbeddedServerData serverData)
    {
        try
        {
            var json = JsonSerializer.Serialize(serverData);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] appendPayload = [.. jsonBytes, .. BitConverter.GetBytes(jsonBytes.Length)];
            var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var appendableStream = new AppendableStream(fs, appendPayload);
            return Task.FromResult(Result.Ok(appendableStream));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Fail<AppendableStream>(ex));
        }
    }
}
