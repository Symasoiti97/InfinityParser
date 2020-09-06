namespace Dto.QueueMessages
{
    public class DistributorMessageDto
    {
        public ParserSiteDto ParserSite { get; set; }
        public dynamic Items { get; set; }
    }
}