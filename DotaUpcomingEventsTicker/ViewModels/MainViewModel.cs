﻿using DotaUpcomingEventsTicker.Api.Models;
using DotaUpcomingEventsTicker.Api.Scraper;
using DotaUpcomingEventsTicker.Commands;
using DotaUpcomingEventsTicker.Enums;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DotaUpcomingEventsTicker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ConcurrentDictionary<string, Match> _matchesSelectedForNotificationList = new ConcurrentDictionary<string,Match>();


        private Timer _timer;
        private TimeSpan TIMER_PERIOD = TimeSpan.FromMinutes(1);
        private TimeSpan TIMER_DUETIME = TimeSpan.FromMinutes(1);

        public event EventHandler<SendNotificationEventArgs> SendNotification;
        public event EventHandler CloseApp;

        #region Public Properties
        private bool _matchesAreLoaded;
        public bool MatchesAreLoaded 
        { 
            get
            {
                return _matchesAreLoaded;
            }
            set
            {
                _matchesAreLoaded = value;
                RaisePropertyChanged("MatchesAreLoaded");
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set 
            { 
                _status = value;
                RaisePropertyChanged();
            }
        }

        public int MatchesSelectedForNotificationCount
        {
            get
            {
                return _matchesSelectedForNotificationList.Count;
            }
        }
        
        public ObservableCollection<Match> Matches { get; set; }

        public IScraper Scraper { get; set; }

        private bool _isParentWindowDeactivated;
        public bool IsParentWindowDeactivated
        {
            get { return _isParentWindowDeactivated; }
            set 
            { 
                _isParentWindowDeactivated = value;
                RaisePropertyChanged();
            }
        }


        private bool _isContextMenuVisible;
        public bool IsContextMenuVisible
        {
            get { return _isContextMenuVisible; }
            set 
            { 
                _isContextMenuVisible = value;
                RaisePropertyChanged();
            }   
        }
        
        #endregion Public Properties

        #region Public Commands
        public IDelegateCommand OnViewLiveMatchCommand { get; private set; }
        public IDelegateCommand OnRefreshMatchesListCommand { get; private set; }
        public IDelegateCommand OnSetNotificationForMatchCommand { get; private set; }
        public IDelegateCommand OnCloseApplicationCommand { get; private set; }
        #endregion Public Commands

        #region Constructors
        public MainViewModel()
        {
            Matches = new ObservableCollection<Match>();
            Scraper = new GosugamersScraper("http://www.gosugamers.net/dota2/gosubet");

            OnViewLiveMatchCommand = new DelegateCommand(OnViewLiveMatch);
            OnRefreshMatchesListCommand = new DelegateCommand(OnRefreshMatches);
            OnSetNotificationForMatchCommand = new DelegateCommand(OnSetNotificationForMatch);
            OnCloseApplicationCommand = new DelegateCommand(OnCloseApplication);

            _timer = new Timer(RunScrapperToLoadMatches, this.Scraper, Timeout.Infinite, Timeout.Infinite);

            this.StartLoadingScrapperAndData();
        }
        #endregion Constructors

        #region Timer EventHandlers
        private void RunScrapperToLoadMatches(object state)
        {
            IScraper scraper = state as IScraper;
            Dispatcher dispatchObject = Application.Current.Dispatcher;

            if (scraper != null)
            {
                dispatchObject.Invoke(() =>
                       {
                           MatchesAreLoaded = false;
                           Status = "Please wait, we are getting the matches..";
                       });

                Scraper.LoadDocument();

                List<Match> parsedMatches = new List<Match>();

                HtmlNodeCollection upcomingMatchesTableRows = Scraper.GetHtmlNodesByXpath(XpathEnums.UpcomingAnchors);
                if (upcomingMatchesTableRows != null)
                {
                    foreach (var item in this.GetMatchesFromAnchorHtmlNodeCollection(upcomingMatchesTableRows))
                    {
                        parsedMatches.Add(item);
                    }
                }

                if (dispatchObject == null || dispatchObject.CheckAccess())
                {
                    this.Matches.Clear();
                    parsedMatches.ForEach(this.Matches.Add);
                    this.MatchesAreLoaded = true;
                }
                else
                {
                    dispatchObject.Invoke(() =>
                        {

                            this.Matches.Clear();
                            foreach (Match match in parsedMatches)
                            {
                                if (_matchesSelectedForNotificationList.ContainsKey(match.Id))
                                {
                                    if (!match.IsLive)
                                    {
                                        match.IsMarkedForNotification = true;
                                    }
                                    else
                                    {
                                        //Show Notification.
                                        this.FireSendNotificationEvent(match);
                                    }
                                }

                                this.Matches.Add(match);
                            }

                            this.MatchesAreLoaded = true;
                            this.Status = string.Empty;
                        });
                }
            }
            else
            {
                dispatchObject.Invoke(() =>
                    {
                        this.MatchesAreLoaded = true;
                        this.Status = "Failed to load matches. Please retry again.";
                    });
            }
        }
        #endregion Timer EventHandlers

        #region Command Handlers
        private void OnViewLiveMatch(object obj)
        {
            Match casted = obj as Match;
            
            if(casted != null)
            {
                string fullUrl = "http://www.gosugamers.net" + casted.StreamUrl;
                System.Diagnostics.Process.Start(fullUrl);
            }
        }
        private void OnRefreshMatches(object obj)
        {
            if (this.MatchesAreLoaded)
            {
                this.StartLoadingScrapperAndData();
            }
            else
            {
                this.Status = "Matches are loading..";
            }
        }
        private void OnSetNotificationForMatch(object obj)
        {
            Match casted = obj as Match;
            if (casted != null)
            {
                if (!_matchesSelectedForNotificationList.ContainsKey(casted.Id))
                {
                    _matchesSelectedForNotificationList.TryAdd(casted.Id,casted);
                }
                else
                {
                    Match outed;
                    _matchesSelectedForNotificationList.TryRemove(casted.Id, out outed);
                }
            }

            RaisePropertyChanged("MatchesSelectedForNotificationCount");
            if (_matchesSelectedForNotificationList.Count > 0)
            {
                _timer.Change(TIMER_DUETIME, TIMER_PERIOD);
            }
            else
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
        private void OnCloseApplication(object obj)
        {
            var handler = this.CloseApp;
            if (handler != null)
            {
                this.CloseApp(this, new EventArgs());
            }
        }
        #endregion Command Handlers

        #region Private Methods
        private async void StartLoadingScrapperAndData()
        {
            //MatchesAreLoaded = false;
            //Status = "Please wait, we are getting the matches..";

            await Task.Run(() => {
                this.RunScrapperToLoadMatches(this.Scraper);
            });

            //this.MatchesAreLoaded = true;
            //this.Status = "Matches data successfully loaded.";
        }
        private Task<List<Match>> GetLiveMatchesAsync()
        {
            Func<List<Match>> func = () =>
            {
                List<Match> parsedMatches = new List<Match>();

                HtmlNodeCollection upcomingMatchesTableRows = Scraper.GetHtmlNodesByXpath(XpathEnums.UpcomingAnchors);
                if (upcomingMatchesTableRows != null)
                {
                    foreach (var item in this.GetMatchesFromAnchorHtmlNodeCollection(upcomingMatchesTableRows))
                    {
                        parsedMatches.Add(item);
                    }
                }

                return parsedMatches;
            };

            return Task.Run<List<Match>>(func);
        }
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
        private void SetMatchTimeElements(Match match , string timeString)
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
        #endregion Private Methods

        #region Event Handlers
        private void FireSendNotificationEvent(Match match)
        {
            var handler = this.SendNotification;
            if (handler != null)
            {
                this.SendNotification(this, new SendNotificationEventArgs(match));
            }
        }
        #endregion Event Handlers

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        _timer.Dispose();
                    }
                }
                _timer = null;
                _disposed = true;
            }
        }
        #endregion IDisposable
    }

    public class SendNotificationEventArgs : EventArgs
    {
        public Match Match { get; set; }
        public SendNotificationEventArgs(Match match)
        {
            Match = match;
        }
    }
}