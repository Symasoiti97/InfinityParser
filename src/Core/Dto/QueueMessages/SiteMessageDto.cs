namespace Dto.QueueMessages
{
    public class SiteMessageDto<T> where T: class
    {
        public SiteDto Site { get; set; }
    }
}