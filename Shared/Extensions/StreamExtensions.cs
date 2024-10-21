namespace Remotely.Shared.Extensions;

public static class StreamExtensions
{
    public static async Task CopyToAsync(this Stream source, Stream destination, Action<int> bytesReadCallback)
    {
        var buffer = new byte[64_000];
        var totalRead = 0;

        int bytesRead;

        while ((bytesRead = await source.ReadAsync(buffer)) != 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;
            bytesReadCallback.Invoke(totalRead);
        }
    }
}
