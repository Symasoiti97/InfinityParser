using System;
using System.Collections.Generic;
using Db.Models.Common;
using Dto.Common;

namespace Dto
{
    public class DataSiteDto
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Url { get; set; }
        public ItemType ItemType { get; set; }
        public Type ItemParentType { get; set; }
        public Type ItemChildType { get; set; }
        public int IntervalFrom { get; set; }
        public int IntervalTo { get; set; }
        public IDictionary<NotificationType, string> Notifications { get; set; }
        public bool IsParseChild { get; set; }
        public IDictionary<string, string[]> IncludeFilter { get; set; }
        public IDictionary<string, string[]> ExcludeFilter { get; set; }
        public ClientDto Client { get; set; }
    }
}