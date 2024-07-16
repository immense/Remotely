using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.Shared.Reactive;

public class RelayCommand : ICommand
{
    private readonly Func<bool> _canExecute;
    private readonly Action _execute;
    public RelayCommand(Action execute)
    {
        _execute = execute;
        _canExecute = () => true;
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
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

public class RelayCommand<T> : ICommand
{
    private readonly Func<T?, bool> _canExecute;
    private readonly Action<T?> _execute;

    public RelayCommand(Action<T?> execute)
    {
        _execute = execute;
        _canExecute = (parameter) => true;
    }

    public RelayCommand(Action<T?> execute, Func<T?, bool> canExecute)
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
            throw new InvalidOperationException(
                "Parameter is not of the correct type.  " +
                $"Expected type {typeof(T)}.  " +
                $"Received type {parameter.GetType()}.");
        }

        return _canExecute.Invoke(typedParam);
    }

    public void Execute(object? parameter)
    {
        if (parameter is null)
        {
            _execute.Invoke(default);
            return;
        }

        if (parameter is not T typedParam)
        {
            throw new InvalidOperationException(
               "Parameter is not of the correct type.  " +
               $"Expected type {typeof(T)}.  " +
               $"Received type {parameter.GetType()}.");
        }

        _execute.Invoke(typedParam);
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
