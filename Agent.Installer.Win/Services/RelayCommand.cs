#nullable enable

using System;
using System.Windows.Input;

namespace Remotely.Agent.Installer.Win.Services;

public class RelayCommand : ICommand
{
    private readonly Action<object> _action;

    private readonly Predicate<object>? _canExecute;

    public RelayCommand(Action<object> action, Predicate<object>? canExecute = null)
    {
        _action = action;
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
        if (_canExecute is null)
        {
            return true;
        }
        return _canExecute.Invoke(parameter);
    }

    public void Execute(object parameter)
    {
        _action?.Invoke(parameter);
    }
}
