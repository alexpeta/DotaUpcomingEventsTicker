using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Enums
{
    public sealed class XpathEnums
    {
        public static readonly string LiveMatchesHeader = @"//*[@id=""col1""]/div[1]/h1";
        public static readonly string LiveMatchesAnchors = @"//*[@id=""col1""]/div[1]/div/table/tbody/tr";

        public static readonly string UpcomingHeader = @"//*[@id=""col1""]/div[1]/h2";
        public static readonly string UpcomingAnchors = @"//*[@id=""col1""]/div[2]/div/table/tbody/tr";


    }
}
