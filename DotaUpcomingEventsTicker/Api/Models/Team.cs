using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Api.Models
{
    public class Team : BaseEntity
    {
        public string ISOCountryCode { get; set; }
        public string Name { get; set; }
        public string ImagePath
        {
            get
            {
                return string.Format("pack://application:,,,/Resources/Flags/{0}.png", ISOCountryCode);   
            }
        }

    }
}
