using DotaUpcomingEventsTicker.Api.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.Api.Scraper
{
    public class GosugamersScraper : IScraper
    {
        private HtmlDocument _webDocument;

        public HtmlWeb WebHelper { get; private set; }
        public HtmlDocument WebDocument
        {
            get
            {
                return _webDocument;
            }
            private set
            {
                _webDocument = value;
            }
        }
        public string Url { get; private set; }

        public GosugamersScraper(string url)
        {
            WebHelper = new HtmlWeb();
            this.Url = url;
        }

        public HtmlNodeCollection GetHtmlNodesByXpath(string xpath)
        {
            HtmlNodeCollection nodes = WebDocument.DocumentNode.SelectNodes(xpath);
            return nodes;
        }

        public HtmlNode GetNodeByXpath(string xpath)
        {
            HtmlNode node = WebDocument.DocumentNode.SelectSingleNode(xpath);
            return node;
        }
        public Task LoadDocumentTask()
        {
            Action loadAction;

            //if(_webDocument != null)
            //{
            //    loadAction = () => { };
            //}
            //else
            { 
                loadAction = () =>
                {
                    _webDocument = WebHelper.Load(Url);
                };
            }

            return Task.Run(loadAction);
        }
        public void LoadDocument()
        {
            _webDocument = WebHelper.Load(Url);
        }
    }
}
