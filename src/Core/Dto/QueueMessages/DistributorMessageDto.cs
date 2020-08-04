using System.Collections.Generic;

namespace Dto.QueueMessages
{
    public class DistributorMessageDto
    {
        public SiteDto Site { get; set; }
        public IEnumerable<ItemDto> Items { get; set; }
    }
}