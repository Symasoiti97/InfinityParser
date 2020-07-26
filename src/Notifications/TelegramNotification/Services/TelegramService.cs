using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramNotification.Services
{
    public class TelegramService : ITelegramService
    {
        private const int PublishItemCount = 20;
        private readonly ILogger<TelegramService> _logger;
        private readonly ITelegramBotClient _telegramBotClient;
        private static readonly int TimePeriod = int.Parse(TimeSpan.FromSeconds(65).TotalMilliseconds.ToString());

        public TelegramService(
            ILogger<TelegramService> logger,
            ITelegramBotClient telegramBotClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        }

        public async Task SendMessages<T>(ChatId chatId, IEnumerable<T> items) where T : class
        {
            if (chatId == null) throw new ArgumentNullException(nameof(chatId));
            if (items == null) throw new ArgumentNullException(nameof(items));
            
            var count = items.Count() / PublishItemCount;

            for (var i = 0; i <= count; i++)
            {
                var notifyItems = items.Skip(i * PublishItemCount).Take(PublishItemCount);

                foreach (var item in notifyItems)
                {
                    try
                    {
                        await _telegramBotClient.SendTextMessageAsync(chatId, (item as ItemDto)?.HtmlMessage(), ParseMode.Html);
                        _logger.LogInformation("Telegram send notification | ChatId: {0}\t NameObject: {1}", chatId.Identifier, typeof(T).Name);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Telegram | ChatId: {0}\t NameObject: {1}", chatId.Identifier, typeof(T).Name);
                    }
                }

                await Task.Delay(TimePeriod);
            }
        }
    }
}