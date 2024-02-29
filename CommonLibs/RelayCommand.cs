using System.Windows.Input;

namespace CommonLibs
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _executeMethod;
        private readonly Func<object?, bool>? _canExecuteMethod;

        public RelayCommand(Action<object?> executeMethod, Func<object?, bool>? canExecuteMethod = null)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecuteMethod is null || _canExecuteMethod(parameter);
        }

        public void Execute(object? parameter)
        {
            _executeMethod(parameter);
        }

        // Do I need to add some more stuff here?
        // Former versions supported something like this ... I did not find a matching solution so far for DOTNET8 WPF
        //  {
        //     add => CommandManager.RequerySuggested += value;
        //     remove => CommandManager.RequerySuggested -= value;
        // }
        public event EventHandler? CanExecuteChanged;
    }
}