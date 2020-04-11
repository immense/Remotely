using System;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.Services
{
    public class Executor : ICommand
    {
        public Executor(Action<object> executeAction, Predicate<object> isExecutable = null)
        {
            ExecuteAction = executeAction;
            IsExecutable = isExecutable;
        }

#pragma warning disable
        public event EventHandler CanExecuteChanged;
#pragma warning restore

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
