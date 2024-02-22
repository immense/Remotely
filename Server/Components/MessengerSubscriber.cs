using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Components;

public class MessengerSubscriber : ComponentBase, IDisposable
{
    protected readonly ConcurrentQueue<IDisposable> _registrations = new();

    [Inject]
    protected IMessenger Messenger { get; init; } = null!;

    public void Dispose()
    {
        while (_registrations.TryDequeue(out var registration))
        {
            try
            {
                registration.Dispose();
            }
            catch { }
        }
        GC.SuppressFinalize(this);
    }

    protected Task Register<TMessage, TChannel>(TChannel channel, RegistrationCallback<TMessage> handler)
        where TMessage : class 
        where TChannel : IEquatable<TChannel>
    {
        var registration = Messenger.Register(this, channel, handler);
        _registrations.Enqueue(registration);
        return Task.CompletedTask;
    }

    protected Task Register<TMessage>(RegistrationCallback<TMessage> handler)
           where TMessage : class
    {
        var registration = Messenger.Register(this, handler);
        _registrations.Enqueue(registration);
        return Task.CompletedTask;
    }
}
