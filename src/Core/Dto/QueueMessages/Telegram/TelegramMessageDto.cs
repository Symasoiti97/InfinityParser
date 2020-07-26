using System.Collections.Generic;

namespace Dto.QueueMessages.Telegram
{
    public class TelegramMessageDto<T>
    {
        public string ChatId { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}