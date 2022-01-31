using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ParserHtml.Services.MaklerMd
{
    public class MaklerMdParser : IParserService<IEnumerable<MaklerListItem>>
    {
        private readonly ILogger<MaklerMdParser> _logger;
        private static readonly Uri BaseUrl = new Uri("https://999.md/");

        public MaklerMdParser(ILogger<MaklerMdParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IEnumerable<MaklerListItem>> ParseHtmlContent(string htmlContent)
        {
            var htmlDocument = GetHtmlDocument(htmlContent);
            try
            {
                var listElement = ParseList(htmlDocument);
                var newsItems = listElement.ChildNodes.Reverse().Select(GetNewsItem)
                    .Where(newsListItem => newsListItem != null).ToArray().AsEnumerable();

                return Task.FromResult(newsItems);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Ошибка парсинга списка для ресурса {BaseUrl}", e);

                return Task.FromResult(Enumerable.Empty<MaklerListItem>());
            }
        }

        private static HtmlDocument GetHtmlDocument(string sourceContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(sourceContent);

            return htmlDocument;
        }

        private MaklerListItem GetNewsItem(HtmlNode htmlItem)
        {
            try
            {
                var parsedItem = ParseItem(htmlItem);
                if (parsedItem == null)
                    return null;

                var parsedItemsSquareAndFloor = ParseItemsSquareAndFloor(parsedItem);

                var newsListItem = new MaklerListItem
                {
                    Text = ParseText(parsedItem),
                    Url = ParseUrl(parsedItem),
                    Phone = ParsePhone(parsedItem),
                    Price = ParsePrice(htmlItem),
                    Floor = ParseFloor(parsedItemsSquareAndFloor),
                    Square = ParseSquare(parsedItemsSquareAndFloor),
                };

                return newsListItem;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message, e);
                return null;
            }
        }

        private HtmlNode ParseList(HtmlDocument htmlDocument)
        {
            return htmlDocument.GetElementbyId("mainAnList")?.Descendants("section").LastOrDefault()?.ChildNodes["div"];
        }

        private HtmlNode ParseItem(HtmlNode htmlItem)
        {
            return htmlItem.ChildNodes.Count == 0
                ? null
                : htmlItem.ChildNodes.SingleOrDefault(c => c.HasClass("ls-detail_infoBlock"));
        }

        private List<HtmlNode> ParseItemsSquareAndFloor(HtmlNode htmlItem)
        {
            return htmlItem?.ChildNodes.SingleOrDefault(i => i.HasClass("subfir"))?.SelectNodes("div")
                ?.Descendants("div").ToList();
        }

        private string ParsePrice(HtmlNode htmlItem)
        {
            return htmlItem?.ChildNodes.SingleOrDefault(i => i.HasClass("ls-detail_controlsBlock"))?
                .ChildNodes["div"]?.ChildNodes["span"]?.InnerText.Trim();
        }

        private string ParseUrl(HtmlNode htmlItem)
        {
            var itemUrl = string.Empty;
            try
            {
                itemUrl = htmlItem.ChildNodes["h3"].ChildNodes["a"].Attributes["href"].Value;
            }
            catch (NullReferenceException e)
            {
                _logger.LogWarning($@"Ошибка парсинга Url для ресурса {BaseUrl}. Узел href не найден", e);
                return itemUrl;
            }

            return BaseUrl + itemUrl;
        }

        private string ParseFloor(IEnumerable<HtmlNode> htmlItems)
        {
            return htmlItems?.SingleOrDefault(d => d.HasClass("field_169"))?.InnerHtml.Trim();
        }

        private string ParseSquare(IEnumerable<HtmlNode> htmlItems)
        {
            return htmlItems?.SingleOrDefault(i => i.HasClass("field_172"))?.InnerText.Trim();
        }

        private string ParseText(HtmlNode htmlItem)
        {
            var text = htmlItem.ChildNodes["h3"].ChildNodes["a"].InnerText.Trim();
            if (string.IsNullOrEmpty(text))
            {
                _logger.LogWarning($"Ошибка парсинга Text для ресурса {BaseUrl}. Text не найден.");
                return string.Empty;
            }

            return text;
        }

        private string ParsePhone(HtmlNode htmlItem)
        {
            var phone = htmlItem.ChildNodes.SingleOrDefault(t => t.HasClass("ls-detail_anData"))?
                .ChildNodes.SingleOrDefault(t => t.HasClass("phone_icon"))?.InnerHtml.Trim();

            if (string.IsNullOrEmpty(phone))
            {
                throw new ArgumentException("Ошибка парсинга элемента phone. Не найден class='phone_icon'/phone=null");
            }

            return FormatPhones(phone);
        }

        private static string FormatPhones(string phones)
        {
            var formatPhones = string.Empty;
            if (!string.IsNullOrEmpty(phones))
            {
                var phonesArr = phones.Replace("-", string.Empty).Split(',');
                foreach (var phone in phonesArr)
                {
                    var stripPrefix0 = StripPrefix(phone.Trim(), "0");
                    var stripPrefix373 = StripPrefix(stripPrefix0, "373");
                    var splitter = string.IsNullOrEmpty(formatPhones) ? string.Empty : ", ";
                    formatPhones = $"{formatPhones}{splitter}{stripPrefix373}";
                }
            }

            return formatPhones;
        }

        private static string StripPrefix(string text, string prefix)
        {
            return text.StartsWith(prefix) ? text.Substring(prefix.Length) : text;
        }
    }
}