using System.Security.Cryptography;
using Remotely.Shared.Extensions;

namespace Remotely.Shared.Tests;

[TestClass]
public class StreamExtensionsTests
{
    [TestMethod]
    public async Task CopyToAsyncTest()
    {
        var bufferSize = 500_000;
        var srcBuffer = new byte[bufferSize];
        RandomNumberGenerator.Fill(srcBuffer);

        using var src = new MemoryStream(srcBuffer);
        using var dst = new MemoryStream();

        var amounts = new List<int>();

        await src.CopyToAsync(dst, bytesRead =>
        {
            amounts.Add(bytesRead);
        });

        Assert.IsTrue(amounts.Any());
        Assert.AreEqual(bufferSize, amounts.Last());
    }
}