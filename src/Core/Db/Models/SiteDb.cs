using System;
using System.Collections.Generic;
using Db.Interfaces;
using Db.Models.Common;

namespace Db.Models
{
    public class SiteDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Url { get; set; }
        public ItemType ItemType { get; set; }
        public string ItemClass { get; set; }
        public int IntervalFrom { get; set; }
        public int IntervalTo { get; set; }
        public string Notifications { get; set; }

        public Guid ClientId { get; set; }
        public virtual ClientDb Client { get; set; }

        public virtual ICollection<ItemDb> Items { get; set; }
    }
}