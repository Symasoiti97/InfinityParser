using System;

namespace Dto.QueueMessages
{
    public class SiteMessageDto
    {
        public SiteDto Site { get; set; }
        public Type ItemType { get; set; }
    }
}