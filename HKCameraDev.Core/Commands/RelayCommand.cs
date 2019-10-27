﻿using System;
using System.Windows.Input;

namespace HKCameraDev.Core.Commands
{
    public class RelayCommand : ICommand
    {
        private Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;
    }
}
