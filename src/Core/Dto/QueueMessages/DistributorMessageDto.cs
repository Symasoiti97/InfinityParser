﻿namespace Dto.QueueMessages
{
    public class DistributorMessageDto
    {
        public SiteDto Site { get; set; }
        public dynamic Items { get; set; }
    }
}