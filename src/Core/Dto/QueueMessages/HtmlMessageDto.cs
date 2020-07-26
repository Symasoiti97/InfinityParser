namespace Dto.QueueMessages
{
    public class HtmlMessageDto<T> where T : class
    {
        public SiteDto Site { get; set; }
        public string HtmlContent { get; set; }
    }
}