using System;
using System.Threading.Tasks;
using Dto.QueueMessages;
using MassTransit;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TelegramNotification.Services;

namespace TelegramNotification.Consumers
{
    public class TelegramNotificationConsumer : IConsumer<TelegramMessageDto>
    {
        private readonly ILogger<TelegramNotificationConsumer> _logger;
        private readonly ITelegramService _telegramService;

        public TelegramNotificationConsumer(
            ILogger<TelegramNotificationConsumer> logger,
            ITelegramService telegramService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telegramService = telegramService ?? throw new ArgumentNullException(nameof(telegramService));
        }

        public async Task Consume(ConsumeContext<TelegramMessageDto> context)
        {
            _logger.LogInformation("{0} - Get Message", typeof(TelegramNotificationConsumer).Name);

            var message = context.Message;
            var item = message.Item.ToObject(message.ParserSite.ItemType);

            await _telegramService.SendMessages(new ChatId(message.ChatId), item);
        }
    }
}