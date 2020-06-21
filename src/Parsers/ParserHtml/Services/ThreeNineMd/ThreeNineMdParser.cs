using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto.ThreeNineMd;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ParserHtml.Services.ThreeNineMd
{
    public class ThreeNineMdParser : IParserService<IEnumerable<ShortThreeNineMdItem>>
    {
        private readonly ILogger<ThreeNineMdParser> _logger;
        private static readonly Uri BaseUrl = new Uri("https://999.md/");

        public ThreeNineMdParser(ILogger<ThreeNineMdParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<ShortThreeNineMdItem>> ParseHtmlContent(string htmlContent)
        {
            return await ParseItem(htmlContent);
        }

        private async Task<IEnumerable<ShortThreeNineMdItem>> ParseItem(string sourceContent)
        {
            return await Task.Run(() =>
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
                    _logger.Log(LogLevel.Error, $"Ошибка парсинга списка для ресурса 'SiteItem.Name'. {e.Message}", DateTime.Now);

                    return Enumerable.Empty<ShortThreeNineMdItem>();
                }
            });
        }
        
        private HtmlDocument GetHtmlDocument(string sourceContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(sourceContent);

            return htmlDocument;
        }
        
        private HtmlNode[] ParseList(HtmlDocument htmlDocument)
        {
            return htmlDocument.GetElementbyId("js-ads-container").Descendants("li").ToArray();
        }

        private ShortThreeNineMdItem GetItem(HtmlNode htmlNode)
        {
            try
            {
                if (htmlNode == null)
                    return null;

                var p = ParserTitle(htmlNode);

                var item = new ShortThreeNineMdItem
                {
                    Title = ParserTitle(htmlNode),
                    Url = ParseUrl(htmlNode),
                    Price = ParsePrice(htmlNode),
                    ShortDescription = ParseDescription(htmlNode)
                };

                return item;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString(), DateTime.Now);
                return null;
            }
        }

        private string ParseUrl(HtmlNode htmlNode)
        {
            try
            {
                var url = htmlNode.SelectSingleNode(".//*[@class='ads-list-photo-item-title ']")
                    .SelectSingleNode("a").Attributes["href"].Value;
                
                return new Uri(BaseUrl, url).ToString();
            }
            catch (NullReferenceException e)
            {
                _logger.Log(LogLevel.Error, $@"Ошибка парсинга Url для ресурса 'SiteItem.Name'. Узел href не найден. {e.Message}",
                    DateTime.Now);
                return string.Empty;
            }
        }

        private string ParserTitle(HtmlNode htmlNode)
        {
            return htmlNode.SelectSingleNode(".//*[@class='ads-list-photo-item-title ']")?.InnerText.Trim();
        }

        private string ParsePrice(HtmlNode htmlNode)
        {
            return htmlNode.SelectSingleNode(".//*[@class='ads-list-photo-item-price-wrapper']")?
                .InnerText.Trim();
        }

        private string ParseDescription(HtmlNode htmlNode)
        {
            return htmlNode.SelectSingleNode(".//*[@class=' ads-list-photo-item-price ']")?
                .SelectSingleNode(".//*[@class='is-offer-type']")
                .ChildNodes["span"]?.InnerText.Trim();
        }
    }
}