using System;
using System.Collections.Generic;
using Db.Models.Common;
using Dto.Common;

namespace Dto
{
    public class SiteDto
    {
        public Guid Id { get; set; }
        public ItemType ItemType { get; set; }
        public string Url { get; set; }
        public IDictionary<NotificationType, string> Notifications { get; set; }
        public Guid ClientId { get; set; }
    }
}