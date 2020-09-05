namespace Dto.QueueMessages
{
    public class TelegramMessageDto : INotificationMessage
    {
        public SiteDto Site { get; set; }
        public string ChatId { get; set; }
        public dynamic Item { get; set; }
    }
}