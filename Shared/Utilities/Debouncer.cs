﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
namespace Remotely.Shared.Utilities;

public static class Debouncer
{
    private static readonly ConcurrentDictionary<object, Timer> _timers = new();
    public static void Debounce(TimeSpan wait, Action action, [CallerMemberName] string key = "")
    {
        if (_timers.TryRemove(key, out var timer))
        {
            timer.Stop();
            timer.Dispose();
        }
        timer = new Timer(wait.TotalMilliseconds)
        {
            AutoReset = false
        };
        timer.Elapsed += (s, e) => action();
        _timers.TryAdd(key, timer);
        timer.Start();
    }

    public static void Debounce(TimeSpan wait, Func<Task> func, [CallerMemberName] string key = "")
    {
        if (_timers.TryRemove(key, out var timer))
        {
            timer.Stop();
            timer.Dispose();
        }
        timer = new Timer(wait.TotalMilliseconds)
        {
            AutoReset = false
        };
        timer.Elapsed += (s, e) => func();
        _timers.TryAdd(key, timer);
        timer.Start();
    }
}