using System;
using System.Collections.Generic;
using Dto.Common;

namespace Dto
{
    public class DataSiteDto
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Url { get; set; }
        public Type ItemParentType { get; set; }
        public Type ItemChildType { get; set; }
        public int IntervalFrom { get; set; }
        public int IntervalTo { get; set; }
        public Dictionary<NotificationType, string> Notifications { get; set; }
        public bool IsParseChild { get; set; }
        public Dictionary<string, string[]> IncludeFilter { get; set; }
        public Dictionary<string, string[]> ExcludeFilter { get; set; }
        public ClientDto Client { get; set; }
    }
}