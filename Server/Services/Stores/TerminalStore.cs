using Remotely.Shared.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Remotely.Server.Services.Stores;

public interface ITerminalStore
{
    event EventHandler? TerminalLinesChanged;
    ConcurrentQueue<TerminalLineItem> TerminalLines { get; }

    void AddTerminalHistory(string content);

    void AddTerminalLine(string content, string className = "", string title = "");

    string GetTerminalHistory(bool forward);

    void InvokeLinesChanged();
}
public class TerminalStore : ITerminalStore
{
    private readonly ConcurrentQueue<string> _terminalHistory = new();
    private int _terminalHistoryIndex = 0;


    public event EventHandler? TerminalLinesChanged;

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

    public void InvokeLinesChanged()
    {
        TerminalLinesChanged?.Invoke(this, EventArgs.Empty);
    }
}
