using Microsoft.AspNetCore.Components;
using Remotely.Server.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IModalService
    {
        event EventHandler ModalShown;
        List<ModalButton> Buttons { get; }
        string[] Body { get; }
        RenderFragment RenderBody { get; }
        string Title { get; }
        Task ShowModal(string title, string[] body, ModalButton[] buttons = null);
        Task ShowModal(string title, RenderFragment body, ModalButton[] buttons = null);
    }

    public class ModalService : IModalService
    {
        private readonly SemaphoreSlim _modalLock = new(1, 1);

        public event EventHandler ModalShown;
        public List<ModalButton> Buttons { get; } = new List<ModalButton>();
        public string[] Body { get; private set; }
        public RenderFragment RenderBody { get; private set; }
        public bool ShowInput { get; private set; }
        public string Title { get; private set; }
        public async Task ShowModal(string title, string[] body, ModalButton[] buttons = null)
        {
            try
            {
                await _modalLock.WaitAsync();
                Title = title;
                Body = body;
                RenderBody = null;
                Buttons.Clear();
                if (buttons is not null)
                {
                    Buttons.AddRange(buttons);
                }
                ModalShown?.Invoke(this, null);
            }
            finally
            {
                _modalLock.Release();
            }
        }

        public async Task ShowModal(string title, RenderFragment body, ModalButton[] buttons = null)
        {
            try
            {
                await _modalLock.WaitAsync();
                Title = title;
                RenderBody = body;
                Body = null;
                Buttons.Clear();
                if (buttons is not null)
                {
                    Buttons.AddRange(buttons);
                }
                ModalShown?.Invoke(this, null);
            }
            finally
            {
                _modalLock.Release();
            }
        }
    }
}
