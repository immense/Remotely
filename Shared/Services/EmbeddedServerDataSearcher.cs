using MessagePack;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Services
{
    public interface IEmbeddedServerDataSearcher
    {
        Task<Result<RewritableStream>> GetRewrittenStream(string filePath, EmbeddedServerData serverData);
        Task<Result<EmbeddedServerData>> TryGetEmbeddedData(string filePath);
    }

    public class EmbeddedServerDataSearcher : IEmbeddedServerDataSearcher
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
                var result = SearchBuffer(fs, AppConstants.EmbeddedImmySignature);
                if (result == -1)
                {
                    return Result.Fail<EmbeddedServerData>("Signature not found in file buffer.");
                }

                var sizeBytes = new byte[sizeof(int)];
                fs.Seek(result, SeekOrigin.Begin);
                await fs.ReadAsync(sizeBytes);
                var dataSize = BitConverter.ToInt32(sizeBytes);

                var dataBlock = new byte[dataSize];
                fs.Seek(result + sizeof(int), SeekOrigin.Begin);
                await fs.ReadAsync(dataBlock);

                var embeddedData = MessagePackSerializer.Deserialize<EmbeddedServerData>(dataBlock);
                return Result.Ok(embeddedData);
            }
            catch (Exception ex)
            {
                return Result.Fail<EmbeddedServerData>(ex);
            }
        }

        public async Task<Result<RewritableStream>> GetRewrittenStream(string filePath, EmbeddedServerData serverData)
        {
            try
            {
                var dataBytes = MessagePackSerializer.Serialize(serverData);
                var sizeBytes = BitConverter.GetBytes(dataBytes.Length);

                var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var result = SearchBuffer(fs, AppConstants.EmbeddedImmySignature);
                if (result == -1)
                {
                    await fs.DisposeAsync();
                    return Result.Fail<RewritableStream>("Signature not found in file buffer.");
                }

                var rewriteMap = new Dictionary<long, byte>();
                var rewriteIndex = result + AppConstants.EmbeddedImmySignature.Length;

                for (var i = 0; i < sizeBytes.Length; i++)
                {
                    rewriteMap.TryAdd(rewriteIndex++, sizeBytes[i]);
                }

                for (var i = 0; i < dataBytes.Length; i++)
                {
                    rewriteMap.TryAdd(rewriteIndex++, sizeBytes[i]);
                }

                var rewriteStream = new RewritableStream(fs, rewriteMap);
                return Result.Ok(rewriteStream);

            }
            catch (Exception ex)
            {
                return Result.Fail<RewritableStream>(ex);
            }
        }

        private int SearchBuffer(FileStream fileStream, byte[] matchPattern)
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
                    return i;
                }
            }
            return -1;
        }
    }
}
