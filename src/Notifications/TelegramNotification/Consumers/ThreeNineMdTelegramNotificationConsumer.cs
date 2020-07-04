using System;
using System.Threading.Tasks;
using Dto.QueueMessages.Telegram;
using MassTransit;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TelegramNotification.Services;

namespace TelegramNotification.Consumers
{
    public class ThreeNineMdTelegramNotificationConsumer : IConsumer<ThreeNineMdTelegramMessageDto>
    {
        private readonly ILogger<ThreeNineMdTelegramNotificationConsumer> _logger;
        private readonly ITelegramService _telegramService;

        public ThreeNineMdTelegramNotificationConsumer(
            ILogger<ThreeNineMdTelegramNotificationConsumer> logger,
            ITelegramService telegramService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telegramService = telegramService ?? throw new ArgumentNullException(nameof(telegramService));
        }

        public Task Consume(ConsumeContext<ThreeNineMdTelegramMessageDto> context)
        {
            _logger.LogInformation("{0} - Get Message", typeof(ThreeNineMdTelegramNotificationConsumer).Name);

            var message = context.Message;

            _telegramService.SendMessages(new ChatId(message.ChatId), message.Items);

            return Task.CompletedTask;
        }
    }
}