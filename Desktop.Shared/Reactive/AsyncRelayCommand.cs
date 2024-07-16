using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.Shared.Reactive;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<bool> _canExecute;
    private readonly Func<Task> _execute;
    public AsyncRelayCommand(Func<Task> execute)
    {
        _execute = execute;
        _canExecute = () => true;
    }

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke();
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T?, bool> _canExecute;
    private readonly Func<T?, Task> _execute;

    public AsyncRelayCommand(Func<T?, Task> execute)
    {
        _execute = execute;
        _canExecute = (parameter) => true;
    }

    public AsyncRelayCommand(Func<T?, Task> execute, Func<T?, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter is null)
        {
            return _canExecute.Invoke(default);
        }

        if (parameter is not T typedParam)
        {
            throw new InvalidOperationException("Paramter is not of the correct type.");
        }

        return _canExecute.Invoke(typedParam);
    }

    // Async void is una*void*able here (heh, heh) due to ICommand's interface.
    // Though we shouldn't need to in modern .NET, we're handling UnobservedTaskException
    // in IServiceProviderExtensions.UseRemoteControl.  In older versions of .NET, this
    // would have been required to prevent the app from terminating.
    public async void Execute(object? parameter)
    {
        if (parameter is null)
        {
            await _execute.Invoke(default);
            return;
        }

        if (parameter is not T typedParam)
        {
            throw new InvalidOperationException("Paramter is not of the correct type.");
        }

        await _execute.Invoke(typedParam);
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
