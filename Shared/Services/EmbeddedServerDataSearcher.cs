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
    string GetEncodedFileName(string filePath, EmbeddedServerData serverData);
    Result<EmbeddedServerData> TryGetEmbeddedData(string filePath);
}

public class EmbeddedServerDataSearcher() : IEmbeddedServerDataSearcher
{
    public static EmbeddedServerDataSearcher Instance { get; } = new();

    public string GetEncodedFileName(string filePath, EmbeddedServerData serverData)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var ext = Path.GetExtension(filePath);

        // Make the base64 string safe file paths and URIs.
        var encodedData = Convert
            .ToBase64String(MessagePackSerializer.Serialize(serverData))
            .Replace("/", "_")
            .Replace("+", "-");

        return $"{fileName}[{encodedData}]{ext}";
    }

    public Result<EmbeddedServerData> TryGetEmbeddedData(string filePath)
    {
        try
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var start = fileName.LastIndexOf('[') + 1;
            var end = fileName.LastIndexOf(']');
            var base64 = fileName[start..end]
                .Replace("_", "/")
                .Replace("-", "+");

            var embeddedData = MessagePackSerializer.Deserialize<EmbeddedServerData>(Convert.FromBase64String(base64));
            return Result.Ok(embeddedData);
        }
        catch (Exception ex)
        {
            return Result.Fail<EmbeddedServerData>(ex);
        }
    }
}
