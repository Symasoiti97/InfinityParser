using System.Collections.Generic;
using Dto.ThreeNineMd;

namespace Dto.QueueMessages
{
    public class DistributorMessageDto
    {
        public SiteDto Site { get; set; }
        public IEnumerable<ShortThreeNineMdItem> ThreeNineMdCollection { get; set; }
    }
}