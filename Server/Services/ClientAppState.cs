using Remotely.Server.Enums;
using Remotely.Shared.Enums;
using Remotely.Shared.Primitives;
using Remotely.Shared.ViewModels;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface IClientAppState : INotifyPropertyChanged, IInvokePropertyChanged
{
    DeviceCardState DevicesFrameFocusedCardState { get; set; }
    string? DevicesFrameFocusedDevice { get; set; }
    ConcurrentList<string> DevicesFrameSelectedDevices { get; }
    ConcurrentQueue<TerminalLineItem> TerminalLines { get; }

    void AddTerminalHistory(string content);

    void AddTerminalLine(string content, string className = "", string title = "");

    string GetTerminalHistory(bool forward);
}

public class ClientAppState : ViewModelBase, IClientAppState
{
    private readonly ConcurrentQueue<string> _terminalHistory = new();
    private int _terminalHistoryIndex = 0;

    public DeviceCardState DevicesFrameFocusedCardState
    {
        get => Get<DeviceCardState>();
        set => Set(value);
    }

    public string? DevicesFrameFocusedDevice
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