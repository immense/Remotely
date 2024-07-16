using Immense.RemoteControl.Shared.Models.Dtos;
using MessagePack;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace Immense.RemoteControl.Shared.Helpers;

public static class DtoChunker
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public static IEnumerable<DtoWrapper> ChunkDto<T>(T dto, DtoType dtoType, string requestId = "", int chunkSize = 50_000)
    {
        var dtoBytes = MessagePackSerializer.Serialize(dto);
        var instanceId = Guid.NewGuid().ToString();
        var chunks = dtoBytes.Chunk(chunkSize).ToArray();

        for (var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];

            yield return new DtoWrapper()
            {
                DtoChunk = chunk,
                DtoType = dtoType,
                SequenceId = i,
                IsFirstChunk = i == 0,
                IsLastChunk = i == chunks.Length - 1,
                RequestId = requestId,
                InstanceId = instanceId
            };
        }
    }

    public static bool TryComplete<T>(DtoWrapper wrapper, [NotNullWhen(true)] out T? result)
    {
        result = default;

        var chunks = _cache.GetOrCreate(wrapper.InstanceId, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);
            return new List<DtoWrapper>();
        });

        if (chunks is null)
        {
            return false;
        }

        lock (chunks)
        {
            chunks.Add(wrapper);

        }

        if (!wrapper.IsLastChunk)
        {
            return false;
        }

        _cache.Remove(wrapper.InstanceId);

        chunks.Sort((a, b) =>
        {
            return a.SequenceId - b.SequenceId;
        });

        var buffer = chunks.SelectMany(x => x.DtoChunk).ToArray();

        result = MessagePackSerializer.Deserialize<T>(buffer);
        return result is not null;
    }
}
