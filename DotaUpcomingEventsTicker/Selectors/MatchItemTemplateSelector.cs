using DotaUpcomingEventsTicker.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DotaUpcomingEventsTicker.Selectors
{
    public class MatchItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LiveMatchDataTemplate { get; set; }
        public DataTemplate UpcomingMatchDataTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Match match = item as Match;

            if (match != null)
            {
                if(match.IsLive)
                {
                    return LiveMatchDataTemplate;
                }
                else
                {
                    return UpcomingMatchDataTemplate;
                }
            }
            else
            {
                return UpcomingMatchDataTemplate;
            }
        }
    }
}
