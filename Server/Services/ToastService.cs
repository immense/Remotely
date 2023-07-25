using Nihs.ConcurrentList;
using Remotely.Server.Enums;
using Remotely.Server.Models;
using System;
using System.Timers;

namespace Remotely.Server.Services;

public interface IToastService
{
    event EventHandler OnToastsChanged;

    ConcurrentList<Toast> Toasts { get; }
    void ShowToast(
        string message, 
        int expirationMillisecond = 3000, 
        string classString = null, 
        string styleOverrides = null);

    void ShowToast2(
        string message,
         ToastType toastType = ToastType.Info,
        int expirationMillisecond = 3000,
        string styleOverrides = null);
}

public class ToastService : IToastService
{
    public event EventHandler OnToastsChanged;
    public ConcurrentList<Toast> Toasts { get; } = new();

    public void ShowToast(string message,
        int expirationMillisecond = 3000,
        string classString = null,
        string styleOverrides = null)
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

        Toasts.Add(toastModel);

        OnToastsChanged?.Invoke(this, EventArgs.Empty);

        var removeToastTimer = new Timer(toastModel.Expiration.TotalMilliseconds + 1000)
        {
            AutoReset = false
        };
        removeToastTimer.Elapsed += (s, e) =>
        {
            Toasts.Remove(toastModel);
            OnToastsChanged?.Invoke(this, null);
            removeToastTimer.Dispose();
        };
        removeToastTimer.Start();
    }

    public void ShowToast2(
        string message, 
        ToastType toastType,
        int expirationMillisecond = 3000, 
        string styleOverrides = null)
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
