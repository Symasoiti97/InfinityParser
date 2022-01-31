using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto.Fl;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ParserHtml.Services.Fl
{
    public class FlParser : IParserService<IEnumerable<FlItem>>
    {
        private readonly ILogger<FlParser> _logger;
        private static readonly Uri BaseUrl = new Uri("https://fl.md/");

        public FlParser(ILogger<FlParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IEnumerable<FlItem>> ParseHtmlContent(string htmlContent)
        {
            return Task.FromResult(ParseItem(htmlContent));
        }

        private IEnumerable<FlItem> ParseItem(string sourceContent)
        {
            var htmlDocument = GetHtmlDocument(sourceContent);
            try
            {
                var listElement = ParseList(htmlDocument);

                var newsItems = listElement.Select(GetItem)
                    .Where(newsListItem => newsListItem != null).ToArray();

                return newsItems;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "{0} - Error parsing list for resource {1}", nameof(FlParser), BaseUrl.ToString());
                return Enumerable.Empty<FlItem>();
            }
        }

        private HtmlDocument GetHtmlDocument(string sourceContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(sourceContent);

            return htmlDocument;
        }

        private static HtmlNode[] ParseList(HtmlDocument htmlDocument)
        {
            var listElement = htmlDocument.GetElementbyId("projects-list").ChildNodes.ToArray();
            return listElement;
        }

        private FlItem GetItem(HtmlNode htmlNode)
        {
            try
            {
                if (htmlNode == null)
                    return null;

                var item = new FlItem
                {
                    Url = ParseUrl(htmlNode),
                    Content = ParseText(htmlNode)
                };

                return item;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing HtmlNode: {0}", htmlNode);
                return null;
            }
        }


        private HtmlNode ParseItem(HtmlNode htmlItem)
        {
            return htmlItem.ChildNodes.Count == 0 || !htmlItem.Id.Contains("project-item")
                ? null
                : htmlItem.ChildNodes["h2"].ChildNodes["a"];
        }

        private static string ParseUrl(HtmlNode htmlItem)
        {
            var itemUrl = htmlItem.Attributes["href"].Value;
            return new Uri(BaseUrl, itemUrl).ToString();
        }

        private static string ParseText(HtmlNode htmlItem)
        {
            return htmlItem.InnerText;
        }
    }
}