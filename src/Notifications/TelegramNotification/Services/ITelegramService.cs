using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramNotification.Services
{
    public interface ITelegramService
    {
        Task SendMessages<T>(ChatId chatId, params T[] obj) where T : class;
    }
}