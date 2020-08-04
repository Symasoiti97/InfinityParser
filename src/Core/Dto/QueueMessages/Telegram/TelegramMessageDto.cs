using System.Collections.Generic;

namespace Dto.QueueMessages.Telegram
{
    public class TelegramMessageDto
    {
        public string ChatId { get; set; }
        public IEnumerable<ItemDto> Items { get; set; }
    }
}