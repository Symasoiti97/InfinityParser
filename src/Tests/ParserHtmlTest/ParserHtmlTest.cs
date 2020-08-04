using System;
using System.Net.Http;
using Dto.Fl;
using Dto.ThreeNineMd;
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

        [Test]
        public void Test_GetTypeName()
        {
            var typeShortThreeNineMd = typeof(ShortThreeNineMdItem);
            var nameShortThreeNineMd = typeShortThreeNineMd.AssemblyQualifiedName;
            var typeShortThreeNineMdByName = Type.GetType(nameShortThreeNineMd);

            var typeFlItem = typeof(FlItem);
            var nameFlItem = typeFlItem.AssemblyQualifiedName;
            var typeFlItemByName = Type.GetType(nameFlItem);

            Assert.Pass();
        }
    }
}