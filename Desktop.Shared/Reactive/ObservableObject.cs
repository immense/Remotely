using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Immense.RemoteControl.Desktop.Shared.Reactive;

public class ObservableObject : INotifyPropertyChanged
{
    private readonly ConcurrentDictionary<string, object?> _backingFields = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected T? Get<T>([CallerMemberName] string propertyName = "")
    {
        if (_backingFields.TryGetValue(propertyName, out var value) &&
            value is T typedValue)
        {
            return typedValue;
        }

        return default;
    }

    protected T Get<T>(T defaultValue, [CallerMemberName] string propertyName = "")
    {
        if (_backingFields.TryGetValue(propertyName, out var value) &&
            value is T typedValue)
        {
            return typedValue;
        }

        return defaultValue;
    }

    protected void Set<T>(T newValue, [CallerMemberName] string propertyName = "")
    {
        _backingFields.AddOrUpdate(propertyName, newValue, (k, v) => newValue);
        NotifyPropertyChanged(propertyName);
    }
}
