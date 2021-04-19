using Remotely.Server.Enums;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.ViewModels;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IClientAppState : INotifyPropertyChanged, IInvokePropertyChanged
    {
        ConcurrentList<ChatSession> DevicesFrameChatSessions { get; }
        DeviceCardState DevicesFrameFocusedCardState { get; set; }
        string DevicesFrameFocusedDevice { get; set; }
        ConcurrentList<string> DevicesFrameSelectedDevices { get; }
        ConcurrentQueue<TerminalLineItem> TerminalLines { get; }

        void AddTerminalHistory(string content);

        void AddTerminalLine(string content, string className = "", string title = "");

        Task<Theme> GetEffectiveTheme();
        string GetTerminalHistory(bool forward);
    }

    public class ClientAppState : ViewModelBase, IClientAppState
    {
        private readonly IApplicationConfig _appConfig;
        private readonly IAuthService _authService;
        private readonly ConcurrentQueue<string> _terminalHistory = new();
        private int _terminalHistoryIndex = 0;

        public ClientAppState(
            IAuthService authService,
            IApplicationConfig appConfig)
        {
            _authService = authService;
            _appConfig = appConfig;
        }

        public ConcurrentList<ChatSession> DevicesFrameChatSessions { get; } = new();

        public DeviceCardState DevicesFrameFocusedCardState
        {
            get => Get<DeviceCardState>();
            set => Set(value);
        }

        public string DevicesFrameFocusedDevice
        {
            get => Get<string>();
            set => Set(value);
        }

        public ConcurrentList<string> DevicesFrameSelectedDevices { get; } = new();

        public ConcurrentQueue<TerminalLineItem> TerminalLines { get; } = new();

        public void AddTerminalHistory(string content)
        {
            while (_terminalHistory.Count > 500)
            {
                _terminalHistory.TryDequeue(out _);
            }

            _terminalHistory.Enqueue(content);
            _terminalHistoryIndex = _terminalHistory.Count;
        }

        public void AddTerminalLine(string content, string className = "", string title = "")
        {
            while (TerminalLines.Count > 500)
            {
                TerminalLines.TryDequeue(out _);
            }

            TerminalLines.Enqueue(new TerminalLineItem()
            {
                Text = content,
                ClassName = className,
                Title = title
            });
        }

        public async Task<Theme> GetEffectiveTheme()
        {
            if (await _authService.IsAuthenticated())
            {
                var user = await _authService.GetUser();
                return user?.UserOptions?.Theme ?? _appConfig.Theme;
            }
            return _appConfig.Theme;
        }

        public string GetTerminalHistory(bool forward)
        {
            if (!_terminalHistory.Any())
            {
                return "";
            }

            if (forward && _terminalHistoryIndex < _terminalHistory.Count)
            {
                _terminalHistoryIndex++;
            }
            else if (!forward && _terminalHistoryIndex > 0)
            {
                _terminalHistoryIndex--;
            }

            if (_terminalHistoryIndex < 0 || _terminalHistoryIndex >= _terminalHistory.Count)
            {
                return "";
            }
            return _terminalHistory.ElementAt(_terminalHistoryIndex);
        }
    }
}