using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Enums;
using System;
using System.Threading.Tasks;

namespace Remotely.Agent.Services;

public interface IScriptingShellFactory
{
    Task<IExternalScriptingShell> GetOrCreateExternalShell(ScriptingShell shell, string senderConnectionId);
    IPsCoreShell GetOrCreatePsCoreShell(string senderConnectionId);
}

internal class ScriptingShellFactory : IScriptingShellFactory
{
    private readonly MemoryCache _sessionCache = new(new MemoryCacheOptions());
    private readonly IServiceProvider _serviceProvider;

    public ScriptingShellFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IPsCoreShell GetOrCreatePsCoreShell(string senderConnectionId)
    {
        if (_sessionCache.TryGetValue(senderConnectionId, out var result) &&
            result is IPsCoreShell shell &&
            !shell.IsDisposed)
        {
            return shell;
        }

        shell = _serviceProvider.GetRequiredService<IPsCoreShell>();
        shell.SenderConnectionId = senderConnectionId;
        _sessionCache.Set(senderConnectionId, shell, GetEntryOptions());
        return shell;
    }

    public async Task<IExternalScriptingShell> GetOrCreateExternalShell(
        ScriptingShell shell,
        string senderConnectionId)
    {
        if (_sessionCache.TryGetValue($"{shell}-{senderConnectionId}", out var result) &&
            result is IExternalScriptingShell scriptingShell &&
            !scriptingShell.IsDisposed)
        {
            return scriptingShell;
        }

        var session = _serviceProvider.GetRequiredService<IExternalScriptingShell>();

        switch (shell)
        {
            case ScriptingShell.WinPS:
                await session.Init(shell, "powershell.exe", "\r\n", senderConnectionId);
                break;
            case ScriptingShell.Bash:
                await session.Init(shell, "bash", "\n", senderConnectionId);
                break;
            case ScriptingShell.CMD:
                await session.Init(shell, "cmd.exe", "\r\n", senderConnectionId);
                break;
            default:
                throw new ArgumentException($"Unknown external scripting shell type: {shell}");
        }

        _sessionCache.Set($"{shell}-{senderConnectionId}", session, GetEntryOptions());
        return session;
    }

    private MemoryCacheEntryOptions GetEntryOptions()
    {
        var options = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };
        options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration()
        {
            EvictionCallback = (key, value, reason, state) =>
            {
                if (value is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch { }
                }
            }
        });

        return options;
    }
}
