using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Primitives;


/// <summary>
/// An implementation of <see cref="IAsyncDisposable"/> that lets you provide a
/// callback, which will be invoked when the object is disposed.
/// </summary>
public sealed class CallbackDisposableAsync : IAsyncDisposable
{
    private readonly Func<ValueTask> _callback;
    private readonly Func<Exception, ValueTask> _exceptionHandler;

    /// <summary>
    /// Create anew instance where exceptions will be caught and suppressed.
    /// </summary>
    /// <param name="callback"></param>
    public CallbackDisposableAsync(Func<ValueTask> callback)
      : this(callback, (_) => ValueTask.CompletedTask)
    {
    }

    /// <summary>
    /// Create a new instance where exceptions will be caught and passed to the supplied handler.
    /// </summary>
    /// <param name="callback"></param>
    public CallbackDisposableAsync(
      Func<ValueTask> callback,
      Func<Exception, ValueTask> exceptionHandler)
    {
        _callback = callback;
        _exceptionHandler = exceptionHandler;
    }


    public ValueTask DisposeAsync()
    {
        try
        {
            return _callback.Invoke();
        }
        catch (Exception ex)
        {
            return _exceptionHandler.Invoke(ex);
        }
    }
}
