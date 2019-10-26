using System;
using System.Windows.Input;

namespace UI.Commands
{
  
    public class SimpleCommand : ICommand
    {
        private Action<object> _execute;

        private Predicate<object> _canExecute;
        private event EventHandler _canExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                _canExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                _canExecuteChanged -= value;
            }
        }
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            if (_canExecuteChanged != null)
                _canExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        public SimpleCommand(Action<object> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = ((obj) => true);
        }
        public SimpleCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            if (canExecute == null)
                throw new ArgumentNullException("canExecute");

            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}