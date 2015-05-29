using DotaUpcomingEventsTicker.Api.Models;
using DotaUpcomingEventsTicker.Api.Scraper;
using DotaUpcomingEventsTicker.Enums;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.DAL
{
    public class MatchRepository : IRepository<Match>
    {
        private IScraper _scraper;
        public IScraper Scraper
        {
            get
            {
                return _scraper;
            }
            private set
            {
                _scraper = value;
            }
        }

        #region Constructors
        public MatchRepository() //IScraper scraper)
        {
            Scraper = new GosugamersScraper("http://www.gosugamers.net/dota2/gosubet");
        }
        #endregion Constructors

        public List<Match> GetList()
        {
            List<Match> parsedMatches = new List<Match>();

            Scraper.LoadDocument();

            HtmlNodeCollection upcomingMatchesTableRows = Scraper.GetHtmlNodesByXpath(XpathEnums.UpcomingAnchors);
            if (upcomingMatchesTableRows != null)
            {
                foreach (var item in this.GetMatchesFromAnchorHtmlNodeCollection(upcomingMatchesTableRows))
                {
                    parsedMatches.Add(item);
                }
            }

            return parsedMatches;
        }
        public Task<List<Match>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        #region Private Matches
        private List<Match> GetMatchesFromAnchorHtmlNodeCollection(HtmlNodeCollection tableRows)
        {
            List<Match> result = new List<Match>();

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;


            int nodeIndex = 1;
            foreach (HtmlNode node in tableRows)
            {
                HtmlNode anchor = node.SelectSingleNode("(//td[1]/a)[" + nodeIndex.ToString() + "]");
                HtmlNode liveInNode = node.SelectSingleNode("(//td[2]/span)[" + nodeIndex.ToString() + "]");

                string url = anchor.Attributes["href"].Value;
                string matchString = url.Substring(url.IndexOf("matches") + 8, url.Length - url.IndexOf("matches") - 8);

                string[] parts = matchString.Split('-');

                Match m = new Match();

                if (liveInNode != null)
                {
                    string timeLeftString = liveInNode.InnerText.Trim();
                    SetMatchTimeElements(m, timeLeftString);
                }

                m.StreamUrl = url;
                m.Id = parts[0];

                Team t1 = new Team();
                Team t2 = new Team();

                string t1FlagSpanClass = anchor.SelectSingleNode("(//span[1]/span[2])[" + nodeIndex.ToString() + "]").Attributes["class"].Value;
                string t2FlagSpanClass = anchor.SelectSingleNode("(//span[5]/span[1])[" + nodeIndex.ToString() + "]").Attributes["class"].Value;

                //*[@id="col1"]/div[2]/div/table/tbody/tr[1]/td[4]/a/span/img
                m.LeagueImage = "http://www.gosugamers.net" + node.SelectSingleNode("(//td[4]/a/span/img)[" + nodeIndex.ToString() + "]").Attributes["src"].Value;

                t1FlagSpanClass = t1FlagSpanClass.Replace("flag ", "");
                t2FlagSpanClass = t2FlagSpanClass.Replace("flag ", "");

                string fullname = string.Empty;
                for (int i = 1; i < parts.Length; i++)
                {
                    if (parts[i] == "vs")
                    {
                        if (!string.IsNullOrWhiteSpace(fullname))
                        {
                            t1.Name = this.SanitizeTeamName(fullname);
                            t1.ISOCountryCode = t1FlagSpanClass;
                            fullname = string.Empty;
                            continue;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(fullname))
                    {
                        fullname += textInfo.ToTitleCase(parts[i]);
                    }
                    else
                    {
                        fullname += " " + textInfo.ToTitleCase(parts[i]);
                    }

                    if (i == parts.Length - 1)
                    {
                        t2.Name = this.SanitizeTeamName(fullname);
                        t2.ISOCountryCode = t2FlagSpanClass;
                    }
                }

                m.Opponent1 = t1;
                m.Opponent2 = t2;

                result.Add(m);
                nodeIndex++;
            }

            return result;
        }
        private void SetMatchTimeElements(Match match, string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
            {
                match.Seconds = 0;
                match.Minutes = 0;
                match.Hours = 0;
                match.Days = 0;
            }
            else
            {
                string[] splitList = timeString.Split(' ');

                foreach (string splitTimeItem in splitList)
                {
                    if (splitTimeItem.Contains("d"))
                    {
                        match.Days = int.Parse(splitTimeItem.Replace("d", ""));
                    }
                    else if (splitTimeItem.Contains("h"))
                    {
                        match.Hours = int.Parse(splitTimeItem.Replace("h", ""));
                    }
                    else if (splitTimeItem.Contains("m"))
                    {
                        match.Minutes = int.Parse(splitTimeItem.Replace("m", ""));
                    }
                    else if (splitTimeItem.Contains("s"))
                    {
                        match.Seconds = int.Parse(splitTimeItem.Replace("s", ""));
                    }
                }
            }
        }
        private string SanitizeTeamName(string teamName)
        {
            teamName = teamName.Replace("Dota 2", "").Replace("Dota2", "").Trim();

            if (teamName.Length < 4)
            {
                teamName = teamName.ToUpper();
            }

            return teamName;
        }
        #endregion Private Matches
    }
}
