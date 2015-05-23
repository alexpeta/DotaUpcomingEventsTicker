using DotaUpcomingEventsTicker.Api.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Api.Scraper
{
    public interface IScraper
    {
        string Url { get; }
        HtmlNode GetNodeByXpath(string xpath);
        HtmlNodeCollection GetHtmlNodesByXpath(string xpath);
        Task LoadDocumentTask();
        void LoadDocument();
    }
}
