using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Commands
{
    public class DelegateCommand : IDelegateCommand
    {
        public event EventHandler CanExecuteChanged;
        Action<object> execute;
        Func<object, bool> canExecute;

        #region Constructors
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
            this.canExecute = this.AlwaysCanExecute;
        }
        #endregion Constructors

        public void Execute(object param)
        {
            execute(param);
        }
        public bool CanExecute(object param)
        {
            return canExecute(param);
        }
        public void RaiseCanExecuteChanged()
        {
            var canExecuteHandler = CanExecuteChanged;
            if (canExecuteHandler != null)
                canExecuteHandler(this, EventArgs.Empty);
        }
        private bool AlwaysCanExecute(object param)
        {
            return true;
        }
    }
}
