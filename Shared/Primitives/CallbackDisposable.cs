using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Primitives;


/// <summary>
/// An implementation of <see cref="IDisposable"/> that lets you provide a
/// callback, which will be invoked when the object is disposed.
/// </summary>
public sealed class CallbackDisposable : IDisposable
{
    private readonly Action _callback;
    private readonly Action<Exception> _exceptionHandler;

    /// <summary>
    /// Create anew instance where exceptions will be caught and suppressed.
    /// </summary>
    /// <param name="callback"></param>
    public CallbackDisposable(Action callback)
      : this(callback, (_) => { })
    {
    }

    /// <summary>
    /// Create a new instance where exceptions will be caught and passed to the supplied handler.
    /// </summary>
    /// <param name="callback"></param>
    public CallbackDisposable(
      Action callback,
      Action<Exception> exceptionHandler)
    {
        _callback = callback;
        _exceptionHandler = exceptionHandler;
    }


    public void Dispose()
    {
        try
        {
            _callback.Invoke();
        }
        catch (Exception ex)
        {
            _exceptionHandler.Invoke(ex);
        }
    }
}
