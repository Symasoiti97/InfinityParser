using System.Collections.Generic;
using Dto.ThreeNineMd;

namespace Dto.QueueMessages.Telegram
{
    public class ThreeNineMdTelegramMessageDto : TelegramMessageDto
    {
        public IEnumerable<ShortThreeNineMdItem> Items { get; set; }
    }
}