namespace Dto.QueueMessages
{
    public class TelegramMessageDto
    {
        public SiteDto Site { get; set; }
        public string ChatId { get; set; }
        public dynamic Items { get; set; }
    }
}