using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Components;

public class MessengerSubscriber : ComponentBase, IAsyncDisposable
{
    protected readonly ConcurrentQueue<IAsyncDisposable> _registrations = new();

    [Inject]
    protected IMessenger Messenger { get; init; } = null!;

    public async ValueTask DisposeAsync()
    {
        while (_registrations.TryDequeue(out var registration))
        {
            try
            {
                await registration.DisposeAsync();
            }
            catch { }
        }
        GC.SuppressFinalize(this);
    }

    protected async Task Register<TMessage, TChannel>(TChannel channel, Func<TMessage, Task> handler)
        where TMessage : class 
        where TChannel : IEquatable<TChannel>
    {
        var registration = await Messenger.Register(this, channel, handler);
        _registrations.Enqueue(registration);
    }

    protected async Task Register<TMessage>(Func<TMessage, Task> handler)
           where TMessage : class
    {
        var registration = await Messenger.Register(this, handler);
        _registrations.Enqueue(registration);
    }
}
