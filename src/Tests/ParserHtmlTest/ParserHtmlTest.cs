using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Db.Models;
using Db.Models.Common;
using DistributorService.Services.Filter;
using Domain;
using Domain.Provider;
using Dto.Common;
using Dto.Fl;
using Dto.ThreeNineMd;
using Helper.Extensions;
using Microsoft.EntityFrameworkCore;
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
        public async Task Test_ParseHtml_ThreeNineMdParser()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var loggerReader = new Logger<ReaderWebClientService>(factory);
            var reader = new ReaderWebClientService(loggerReader, new HttpClient());

            var loggerParser = new Logger<ThreeNineMdParser>(factory);
            var parser = new ThreeNineMdParser(loggerParser);

            var content = await reader.GetAsync("https://999.md/ru/list/transport/cars?selected_currency=original");
            var items = await parser.ParseHtmlContent(content);

            Assert.Pass();
        }

        [Test]
        public async Task Test_GenerateDataInBase()
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<InfinityParserDbContext>();
            contextOptionsBuilder.UseNpgsql(
                "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=InfinityParser;Pooling=true;MinPoolSize=15;MaxPoolSize=20;CommandTimeout=20;Timeout=20");
            await using var dbContext = new InfinityParserDbContext(contextOptionsBuilder.Options);

            var serviceProvider = new ServiceCollection()
                .AddScoped(provider => dbContext)
                .AddScoped<IDataProvider, DataProvider>()
                .BuildServiceProvider();

            var dataProvider = serviceProvider.GetService<IDataProvider>();

            var client = new ClientDb
            {
                Id = Guid.NewGuid(),
                Name = "Test Client"
            };

            var sites = new[]
            {
                new SiteDb
                {
                    Id = Guid.NewGuid(),
                    BaseUrl = "https://999.md",
                    Url = "https://999.md/ru/list/transport/cars?selected_currency=original",
                    ItemType = ItemType.Automobile,
                    ItemChildType = null,
                    ItemParentType = typeof(ShortThreeNineMdItem).AssemblyQualifiedName
                }
            };

            var parserSites = new[]
            {
                new ParserSiteDb
                {
                    Id = Guid.NewGuid(),
                    SiteId = sites.Single(i => i.ItemParentType == typeof(ShortThreeNineMdItem).AssemblyQualifiedName).Id,
                    ClientId = client.Id,
                    ExcludeFilter = null,
                    IncludeFilter = null,
                    IntervalFrom = 100,
                    IntervalTo = 200,
                    IsChildParse = false,
                    Notifications = new Dictionary<NotificationType, string>
                    {
                        {NotificationType.Telegram, "-302185881"}
                    }.ToJson()
                }
            };

            using var tr = dataProvider.Transaction();

            await dataProvider.Insert(client);
            await dataProvider.InsertRange(sites);
            await dataProvider.InsertRange(parserSites);

            tr.Complete();
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

        [Test]
        public void Test_Filter_CollectionsEquals()
        {
            var filterService = new FilterService();

            var items = new[]
            {
                new FlItem
                {
                    Content = "Content",
                    Url = "https://fl.ru/items/new?type=work"
                }
            };
            var includeFilter = new Dictionary<string, string[]>
            {
                {"Content", new[] {"Content", "ent", "cont"}}
            };
            var excludeFilter = new Dictionary<string, string[]>
            {
                {"Content", new[] {"Contents"}}
            };

            var itemsByFilter = filterService.Filter(items, includeFilter, excludeFilter);

            CollectionAssert.AreEqual(itemsByFilter, items);
        }
        
        [Test]
        public void Test_Filter_CollectionsNotEquals()
        {
            var filterService = new FilterService();

            var items = new[]
            {
                new FlItem
                {
                    Content = "Content",
                    Url = "https://fl.ru/items/new?type=work"
                }
            };
            var includeFilter = new Dictionary<string, string[]>
            {
                {"Content", new[] {"Contents", "ent", "cont"}}
            };
            var excludeFilter = new Dictionary<string, string[]>
            {
                {"Content", new[] {"Contents"}}
            };

            var itemsByFilter = filterService.Filter(items, includeFilter, excludeFilter);

            CollectionAssert.AreNotEqual(itemsByFilter, items);
        }
    }
}