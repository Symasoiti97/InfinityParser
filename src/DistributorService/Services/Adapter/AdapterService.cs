using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Db.Models;
using DistributorService.Services.Cache;
using Domain.Provider;
using Dto;
using Dto.Common;
using Dto.QueueMessages;
using Helper.Exceptions;
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

        public AdapterService(
            ILogger<AdapterService> logger,
            IDataProvider dataProvider,
            IPublishEndpoint publishEndpoint,
            ICacheService cacheService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task SaveAndPublishNotify<T>(SiteDto site, IEnumerable<T> items) where T : ItemDto
        {
            if (site == null) throw new ArgumentNullException(nameof(site));

            var uncachedItems = _cacheService.SaveAndGetUncached(site.Id, items).ToArray();

            var notifyItems = Array.Empty<T>();

            if (uncachedItems.Any())
            {
                notifyItems = (await InsertAndGetItems(site.Id, uncachedItems)).ToArray();
            }

            if (notifyItems.Any())
            {
                await PublishNotifications(notifyItems, site.Notifications, site);
            }
        }

        private async Task PublishNotifications(IReadOnlyCollection<ItemDto> items, IDictionary<NotificationType, string> notifications, SiteDto site)
        {
            foreach (var (key, value) in notifications)
            {
                switch (key)
                {
                    case NotificationType.Telegram:
                        await TelegramServicePublish(value, items, site);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task TelegramServicePublish(string chatId, IReadOnlyCollection<ItemDto> items, SiteDto site)
        {
            var message = new TelegramMessageDto
            {
                ChatId = chatId,
                Items = items,
                Site = site
            };

            await _publishEndpoint.Publish(message);
            _logger.LogInformation("Telegram service publish ChatId: {0}\tType: {1}\tCount item: {2}", chatId, nameof(items), items.Count);
        }

        private async Task<IEnumerable<T>> InsertAndGetItems<T>(Guid siteId, IEnumerable<T> items) where T : ItemDto
        {
            var insertItem = new List<T>();

            foreach (var itemDto in items)
            {
                var item = new ItemDb
                {
                    SiteId = siteId,
                    Url = itemDto.Url
                };

                try
                {
                    await _dataProvider.Insert(item);
                    insertItem.Add(itemDto);
                }
                catch (Exception e)
                {
                    if (!(e is ObjectAlreadyExistsException))
                    {
                        _logger.LogError(e, "Error inserting row in data base.");
                    }
                }
            }

            return insertItem;
        }
    }
}