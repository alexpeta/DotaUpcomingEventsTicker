using DotaUpcomingEventsTicker.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.ViewModels
{
    public class NotificationViewModel: ViewModelBase
    {
        public event EventHandler CloseApp;

        #region Public Commands
        public IDelegateCommand OnCloseApplicationCommand { get; private set; }
        #endregion Public Commands

        #region Constructors
        public NotificationViewModel ()
	    {
            OnCloseApplicationCommand = new DelegateCommand(OnCloseApplication);
	    }
        #endregion Constructors

        #region Private Command Handlers
        private void OnCloseApplication(object obj)
        {
            var handler = this.CloseApp;
            if (handler != null)
            {
                this.CloseApp(this, new EventArgs());
            }
        }
        #endregion Private Command Handlers

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            ;
        }
        #endregion IDisposable
    }
}
