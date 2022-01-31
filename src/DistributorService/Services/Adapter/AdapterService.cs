using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Db.Exceptions;
using Db.Models;
using Db.Provider;
using DistributorService.Services.Cache;
using DistributorService.Services.Filter;
using Dto;
using Dto.Common;
using Dto.QueueMessages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DistributorService.Services.Adapter
{
    public class AdapterService : IAdapterService
    {
        private readonly ILogger<AdapterService> _logger;
        private readonly IDataProvider _dataProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly IFilterService _filterService;

        public AdapterService(
            ILogger<AdapterService> logger,
            IDataProvider dataProvider,
            IPublishEndpoint publishEndpoint,
            ICacheService cacheService,
            IMapper mapper,
            IFilterService filterService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
        }

        public async Task SaveAndPublishNotify<T>(ParserSiteDto parserSite, IEnumerable<T> items) where T : ItemDto
        {
            if (parserSite == null) throw new ArgumentNullException(nameof(parserSite));

            var itemArray = items as T[] ?? items.ToArray();
            var uncachedItems = _cacheService.SaveAndGetUncachedItem(parserSite.Id, itemArray).ToArray();

            var dataSite = GetDataSiteFromCacheOrDataBase(parserSite.Id);
            var uncachedItemWithFilter = _filterService.Filter(itemArray, dataSite.IncludeFilter, dataSite.ExcludeFilter);

            if (uncachedItemWithFilter.Any())
            {
                using var tr = _dataProvider.Transaction();

                await SaveAndPublishNotificationOrParseChild(dataSite, parserSite, uncachedItems);

                tr.Complete();
            }
        }

        private async Task SaveAndPublishNotificationOrParseChild<T>(DataSiteDto dataSite, ParserSiteDto parserSite, IEnumerable<T> items) where T : ItemDto
        {
            foreach (var itemDto in items)
            {
                try
                {
                    var item = new ItemDb
                    {
                        ParserSiteId = parserSite.Id,
                        Url = itemDto.Url
                    };

                    if (dataSite.IsParseChild && dataSite.ItemParentType == parserSite.ItemType)
                    {
                        await PublishForParseChildItem(itemDto, dataSite);
                    }
                    else
                    {
                        await _dataProvider.Insert(item);
                        await PublishNotifications(itemDto, parserSite, dataSite);
                    }
                }
                catch (Exception e)
                {
                    if (e is ObjectAlreadyExistsException)
                    {
                        continue;
                    }

                    _logger.LogError(e, "Error inserting row in data base.");
                }
            }
        }

        private async Task PublishForParseChildItem(ItemDto item, DataSiteDto dataSite)
        {
            var childSite = new ParserSiteDto
            {
                Id = dataSite.Id,
                Url = item.Url,
                ItemType = dataSite.ItemChildType
            };

            var publishMessage = new SiteMessageDto
            {
                ParserSite = childSite
            };
            await _publishEndpoint.Publish(publishMessage);
        }

        private async Task PublishNotifications(ItemDto item, ParserSiteDto parserSiteDto, DataSiteDto dataDataSite)
        {
            foreach (var (key, value) in dataDataSite.Notifications)
            {
                switch (key)
                {
                    case NotificationType.Telegram:
                        await TelegramServicePublish(value, item, parserSiteDto);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task TelegramServicePublish(string chatId, ItemDto item, ParserSiteDto parserSite)
        {
            var message = new TelegramMessageDto
            {
                ChatId = chatId,
                Item = item,
                ParserSite = parserSite
            };

            await _publishEndpoint.Publish(message);
            _logger.LogInformation("Telegram service publish ChatId: {0}\tType: {1}\tItem: {2}", chatId, nameof(item), item.ToString());
        }

        private DataSiteDto GetDataSiteFromCacheOrDataBase(Guid parserSiteId)
        {
            var site = _cacheService.GetCachedSiteOrNull(parserSiteId);

            if (site == null)
            {
                site = _dataProvider.Get<ParserSiteDb>(i => i.Id == parserSiteId).ProjectTo<DataSiteDto>(_mapper.ConfigurationProvider).Single();
                _cacheService.SetCachedSite(site);
            }

            return site;
        }
    }
}