using System;
using System.Collections.Generic;
using Db.Models.Common;
using Dto.Common;

namespace Dto
{
    public class SiteDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public ItemType ItemType { get; set; }
        public Type ItemClass { get; set; }
        public int IntervalFrom { get; set; }
        public int IntervalTo { get; set; }
        public IDictionary<NotificationType, string> Notifications { get; set; }
        public ClientDto Client { get; set; }
    }
}