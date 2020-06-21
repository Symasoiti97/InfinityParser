using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ParserHtml.Services.ThreeNineMd;
using ReaderHtml.Services;

namespace ParserHtmlTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_ParseHtml_ThreeNineMdParser()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var loggerReader = new Logger<ReaderWebClientService>(factory);
            var reader = new ReaderWebClientService(loggerReader, new HttpClient());

            var loggerParser = new Logger<ThreeNineMdParser>(factory);
            var parser = new ThreeNineMdParser(loggerParser);

            var content = reader.GetAsync("https://999.md/ru/list/transport/cars?selected_currency=original").Result;        
            var items = parser.ParseHtmlContent(content).Result;

            Assert.Pass();
        }
    }
}