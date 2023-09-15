using System;
using System.Windows.Input;

namespace CardTerminalSimulator.ViewModel
{
    public class RelayCommand : ICommand
    {
        private Action<object> _executeMethod;
        private Func<object, bool> _canExecuteMethod;

        public RelayCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod(parameter);
        }

        public void Execute(object? parameter)
        {
            _executeMethod(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}