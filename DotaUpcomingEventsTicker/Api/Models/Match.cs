using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Api.Models
{
    public class Match : BaseEntity
    {
        public string TimeToMatchLeft 
        {
            get
            {
                string result = string.Empty;

                if(Days == 0 && Minutes == 0 && Seconds == 0 && Hours == 0)
                {
                    result = "WATCH";
                }
                else
                { 
                    if(Days > 0)
                    {
                        result = string.Format("{0}d {1}h", Days,Hours);
                    }
                    else
                    {
                        result =  string.Format("{0}h {1}m", Hours,Minutes);
                    }
                }
                return result;
            }
        }

        private bool _isMarkedForNotifcation;
        public bool IsMarkedForNotification
        {
            get { return _isMarkedForNotifcation; }
            set 
            { 
                _isMarkedForNotifcation = value;
                RaisePropertyChanged();
            }
        }
        
        public int _seconds;
        public int Seconds 
        { 
            get
            {
                return _seconds;
            }
            set
            {
                _seconds = value;
                RaisePropertyChanged();
                RaisePropertyChanged("TimeToMatchLeft");
            }
        }

        public int _minutes;
        public int Minutes 
        { 
            get
            {
                return _minutes;
            }
            set
            {
                _minutes = value;
                RaisePropertyChanged();
                RaisePropertyChanged("TimeToMatchLeft");
            }
        }

        public int _hours;
        public int Hours 
        { 
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;
                RaisePropertyChanged();
                RaisePropertyChanged("TimeToMatchLeft");
            }
        }

        public int _days;
        public int Days 
        { 
            get
            {
                return _days;
            }
            set
            {
                _days = value;
                RaisePropertyChanged();
                RaisePropertyChanged("TimeToMatchLeft");
            }
        }

        public string Id { get; set; }
        public Team Opponent1 { get; set; }
        public Team Opponent2 { get; set; }
        public string StreamUrl { get; set; }
        public string LeagueImage { get; set; }

        public bool IsLive
        {
            get
            {
                return string.Equals("WATCH", TimeToMatchLeft, StringComparison.CurrentCultureIgnoreCase);
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            Match casted = obj as Match;
            if (casted == null)
            {
                return false;
            }

            return casted.Id == this.Id;
        }
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Opponent1.GetHashCode();
            hash = (hash * 11) + Opponent2.GetHashCode();
            return hash;
        }
    }

}
