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

            if (uncachedItems.Any())
            {
                using var tr = _dataProvider.Transaction();

                await SaveAndPublishNotification(site, uncachedItems);

                tr.Complete();
            }
        }

        private async Task SaveAndPublishNotification<T>(SiteDto site, IEnumerable<T> items) where T : ItemDto
        {
            foreach (var itemDto in items)
            {
                var item = new ItemDb
                {
                    SiteId = site.Id,
                    Url = itemDto.Url
                };

                try
                {
                    await _dataProvider.Insert(item);
                    await PublishNotifications(itemDto, site);
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

        private async Task PublishNotifications(ItemDto item, SiteDto site)
        {
            foreach (var (key, value) in site.Notifications)
            {
                switch (key)
                {
                    case NotificationType.Telegram:
                        await TelegramServicePublish(value, item, site);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task TelegramServicePublish(string chatId, ItemDto item, SiteDto site)
        {
            var message = new TelegramMessageDto
            {
                ChatId = chatId,
                Item = item,
                Site = site
            };

            await _publishEndpoint.Publish(message);
            _logger.LogInformation("Telegram service publish ChatId: {0}\tType: {1}\tItem: {2}", chatId, nameof(item), item.ToString());
        }
    }
}