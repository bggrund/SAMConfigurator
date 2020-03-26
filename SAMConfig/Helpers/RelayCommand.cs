using System;
using System.Windows.Input;

namespace SAMConfig
{
   class RelayCommand : ICommand
   {
      private Action<object> action;

      public event EventHandler CanExecuteChanged;

      public bool CanExecute(object parameter) => true;

      public void Execute(object parameter) => action(parameter);

      public RelayCommand(Action<object> a)
      {
         action = a;
      }
   }
}