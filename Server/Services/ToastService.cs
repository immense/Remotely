using Remotely.Server.Enums;
using Remotely.Server.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Remotely.Server.Services;

public interface IToastService
{
    event EventHandler OnToastsChanged;

    IEnumerable<Toast> Toasts { get; }
    void ShowToast(
        string message, 
        int expirationMillisecond = 3000, 
        string classString = "", 
        string styleOverrides = "");

    void ShowToast2(
        string message,
         ToastType toastType = ToastType.Info,
        int expirationMillisecond = 3000,
        string styleOverrides = "");
}

public class ToastService : IToastService
{
    private readonly object _lock = new();
    private readonly List<Toast> _toasts = new();
    public event EventHandler? OnToastsChanged;

    public IEnumerable<Toast> Toasts
    {
        get
        {
            lock (_lock)
            {
                return _toasts.ToArray();
            }
        }
    }

    public void ShowToast(string message,
        int expirationMillisecond = 3000,
        string classString = "",
        string styleOverrides = "")
    {

        if (string.IsNullOrWhiteSpace(classString))
        {
            classString = "bg-success text-white";
        };

        var toastModel = new Toast(Guid.NewGuid().ToString(), 
            message,
            classString, 
            TimeSpan.FromMilliseconds(expirationMillisecond),
            styleOverrides);

        lock (_lock)
        {
            _toasts.Add(toastModel);
        }

        OnToastsChanged?.Invoke(this, EventArgs.Empty);

        var removeToastTimer = new System.Timers.Timer(toastModel.Expiration.TotalMilliseconds + 1000)
        {
            AutoReset = false
        };
        removeToastTimer.Elapsed += (s, e) =>
        {
            lock (_lock)
            {
                _toasts.Remove(toastModel);
            }
            OnToastsChanged?.Invoke(this, EventArgs.Empty);
            removeToastTimer.Dispose();
        };
        removeToastTimer.Start();
    }

    public void ShowToast2(
        string message, 
        ToastType toastType,
        int expirationMillisecond = 3000, 
        string styleOverrides = "")
    {
        var classString = toastType switch
        {
            ToastType.Info => "bg-info text-white",
            ToastType.Success => "bg-success text-white",
            ToastType.Warning => "bg-warning text-white",
            ToastType.Error => "bg-danger text-white",
            _ => "bg-info text-white"
        };

        ShowToast(message, expirationMillisecond, classString, styleOverrides);
    }
}
