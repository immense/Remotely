using Remotely.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Remotely.Server.Services
{
    public interface IToastService
    {
        ConcurrentList<Toast> Toasts { get; }

        event EventHandler OnToastsChanged;

        void ShowToast(string message, int expirationMillisecond = 3000, string classString = null, string styleOverrides = null);
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
    }
}
