using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Remotely.Agent.Installer.Models;
using Remotely.Agent.Installer.Win.Utilities;
using Remotely.Shared;

namespace Remotely.Agent.Installer.Win.Services;

internal class EmbeddedServerDataReader
{
    private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

    public Task<EmbeddedServerData> TryGetEmbeddedData(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"File path does not exist: {filePath}");
            }

            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = SearchBuffer(fs, AppConstants.EmbeddedImmySignature);
                if (result == -1)
                {
                    throw new Exception("Signature not found in file buffer.");
                }

                Logger.Write($"Found data signature at index {result}.");

                fs.Seek(result + AppConstants.EmbeddedImmySignature.Length, SeekOrigin.Begin);
                using (var reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    var serializedData = reader.ReadString();

                    Logger.Write($"Extracted embedded data from EXE: {serializedData}");

                    var embeddedData = _serializer.Deserialize<EmbeddedServerData>(serializedData);
                    if (embeddedData != null)
                    {
                        return Task.FromResult(embeddedData);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Write(ex);
        }
        return Task.FromResult(EmbeddedServerData.Empty);
    }

    private long SearchBuffer(FileStream fileStream, byte[] matchPattern)
    {
        var matchSize = matchPattern.Length;
        var limit = fileStream.Length - matchSize;

        for (var i = 0; i <= limit; i++)
        {
            var k = 0;

            for (; k < matchSize; k++)
            {
                if (matchPattern[k] != fileStream.ReadByte())
                {
                    break;
                }
            }

            if (k == matchSize)
            {
                return fileStream.Position - matchSize;
            }
        }
        return -1;
    }
}
