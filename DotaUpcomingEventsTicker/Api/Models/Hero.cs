using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Api.Models
{
    public class Hero : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
