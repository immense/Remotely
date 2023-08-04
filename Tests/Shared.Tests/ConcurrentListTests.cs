using Remotely.Shared.Primitives;

namespace Remotely.Shared.Tests;

[TestClass]
public class ConcurrentListTests
{
    private readonly int _startCount = 500_000;
    private ConcurrentList<int> _list = new();

    [TestInitialize]
    public void Setup()
    {
        _list = new ConcurrentList<int>();
        for (var i = 0; i < _startCount; i++)
        {
            _list.Add(i);
        }
    }


    [TestMethod]
    public void MultipleOperations_GivenMultipleThreads_Ok()
    {
        Assert.IsTrue(_list.Contains(500));
        Assert.IsTrue(_list.Contains(100_002));

        var reset1 = new ManualResetEvent(false);
        var reset2 = new ManualResetEvent(false);
        var exceptions = 0;

        // Add and remove items from two separate background threads.
        _ = Task.Run(() =>
        {
            for (var i = 0; i < 5_000; i++)
            {
                try
                {
                    Assert.IsTrue(_list.Remove(500 + i));
                    _list.RemoveAt(100_000);
                    _list.Add(42);
                    _list.Insert(200_000, 100);
                }
                catch
                {
                    Interlocked.Increment(ref exceptions);
                }
            }
            reset1.Set();
        });

        _ = Task.Run(() =>
        {
            for (var i = 5_000; i < 10_000; i++)
            {
                try
                {
                    Assert.IsTrue(_list.Remove(500 + i));
                    _list.RemoveAt(100_000);
                    _list.Add(42);
                    _list.Insert(200_000, 100);
                }
                catch
                {
                    Interlocked.Increment(ref exceptions);
                }
            }
            reset2.Set();
        });

        reset1.WaitOne();
        reset2.WaitOne();

        Assert.IsFalse(_list.Contains(500));
        Assert.IsFalse(_list.Contains(100_002));
        // We should have the original count with which we started.
        Assert.AreEqual(_startCount, _list.Count);
        Assert.AreEqual(0, exceptions);
    }
}