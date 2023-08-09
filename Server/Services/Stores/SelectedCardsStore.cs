using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotely.Server.Services.Stores;

public interface ISelectedCardsStore
{
    event EventHandler? SelectedDevicesChanged;
    IEnumerable<string> SelectedDevices { get; }
    bool Add(string deviceId);
    bool Contains(string deviceId);

    bool Remove(string deviceId);
    void Clear();
    void InvokeSelectionsChanged();
}

public class SelectedCardsStore : ISelectedCardsStore
{
    private readonly object _lock = new();
    private readonly HashSet<string> _selectedDevices = new();

    public IEnumerable<string> SelectedDevices
    {
        get
        {
            lock (_lock)
            {
                return _selectedDevices.ToArray();
            }
        }
    }

    public event EventHandler? SelectedDevicesChanged;

    public bool Add(string deviceId)
    {
        lock (_lock)
        {
            return _selectedDevices.Add(deviceId);
        }
    }

    public void Clear()
    {
        _selectedDevices.Clear();
    }

    public bool Contains(string deviceId)
    {
        lock ( _lock)
        {
              return _selectedDevices.Contains(deviceId);
        }
    }

    public void InvokeSelectionsChanged()
    {
        SelectedDevicesChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool Remove(string deviceId)
    {
        lock (_lock)
        {
            return _selectedDevices.Remove(deviceId);
        }
    }
}