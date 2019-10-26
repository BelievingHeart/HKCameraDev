using System;
using System.Windows.Input;

namespace UI.Commands
{
    public class ParameterizedCommand : ICommand
    {
        private Action<Object> _execute;

        public ParameterizedCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
