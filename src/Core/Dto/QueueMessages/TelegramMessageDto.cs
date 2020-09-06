namespace Dto.QueueMessages
{
    public class TelegramMessageDto
    {
        public ParserSiteDto ParserSite { get; set; }
        public string ChatId { get; set; }
        public dynamic Item { get; set; }
    }
}