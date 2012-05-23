using System;
using System.Windows.Input;

namespace BeaverMusic.UI
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _onExecuted;

        public DelegateCommand(Action onExecuted)
        {
            _onExecuted = onExecuted;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _onExecuted();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _onExecuted;

        public DelegateCommand(Action<T> onExecuted)
        {
            _onExecuted = onExecuted;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _onExecuted((T)parameter);
        }
    }
}
