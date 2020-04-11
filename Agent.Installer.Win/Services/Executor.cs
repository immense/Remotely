using System;
using System.Windows.Input;

namespace Remotely.Agent.Installer.Win.Services
{
    public class Executor : ICommand
    {
        public Executor(Action<object> executeAction, Predicate<object> isExecutable = null)
        {
            ExecuteAction = executeAction;
            IsExecutable = isExecutable;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private Action<object> ExecuteAction { get; set; }

        private Predicate<object> IsExecutable { get; set; }
        public bool CanExecute(object parameter)
        {
            if (IsExecutable == null)
            {
                return true;
            }
            return IsExecutable.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteAction.Invoke(parameter);
        }
    }
}
