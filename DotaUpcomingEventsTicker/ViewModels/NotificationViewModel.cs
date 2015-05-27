using DotaUpcomingEventsTicker.Api.Models;
using DotaUpcomingEventsTicker.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.ViewModels
{
    public class NotificationViewModel: ViewModelBase
    {
        public event EventHandler<SendNotificationEventArgs> CloseNotification;

        private Match _match;
        public Match Match 
        {
            get
            {
                return _match;
            }
            set
            {
                _match = value;
                RaisePropertyChanged();
            }
        }

        #region Public Commands
        public IDelegateCommand OnCloseNotificationCommand { get; private set; }
        public IDelegateCommand OnViewLiveMatchCommand { get; private set; }
        #endregion Public Commands

        #region Constructors
        public NotificationViewModel ()
	    {
            OnCloseNotificationCommand = new DelegateCommand(OnCloseNotification);
            OnViewLiveMatchCommand = new DelegateCommand(OnViewLiveMatch);
	    }
        #endregion Constructors

        #region Private Command Handlers
        private void OnCloseNotification(object obj)
        {
            Match casted = obj as Match;
            if(casted != null)
            { 
                var handler = this.CloseNotification;
                if (handler != null)
                {
                    this.CloseNotification(this, new SendNotificationEventArgs(casted));
                }
            }
        }
        private void OnViewLiveMatch(object obj)
        {
            Match casted = obj as Match;

            if (casted != null)
            {
                string fullUrl = "http://www.gosugamers.net" + casted.StreamUrl;
                System.Diagnostics.Process.Start(fullUrl);
            }

            this.OnCloseNotification(obj);
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
