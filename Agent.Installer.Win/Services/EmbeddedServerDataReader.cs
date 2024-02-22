using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Remotely.Agent.Installer.Models;
using Remotely.Agent.Installer.Win.Utilities;

namespace Remotely.Agent.Installer.Win.Services;

internal class EmbeddedServerDataReader
{
    private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

    public async Task<EmbeddedServerData> TryGetEmbeddedData(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"File path does not exist: {filePath}");
            }

            using var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var br = new BinaryReader(fs);
            using var sr = new StreamReader(fs);

            fs.Seek(-4, SeekOrigin.End);
            var dataSize = br.ReadInt32();
            fs.Seek(-dataSize - 4, SeekOrigin.End);

            if (dataSize == 0)
            {
                return EmbeddedServerData.Empty;
            }

            var buffer = new byte[dataSize];
            await fs.ReadAsync(buffer, 0, dataSize);
            var json = Encoding.UTF8.GetString(buffer);

            Logger.Write($"Extracted embedded data from EXE: {json}");

            var embeddedData = _serializer.Deserialize<EmbeddedServerData>(json);
            if (embeddedData is not null)
            {
                return embeddedData;
            }
        }
        catch (Exception ex)
        {
            Logger.Write(ex);
        }
        return EmbeddedServerData.Empty;
    }
}
