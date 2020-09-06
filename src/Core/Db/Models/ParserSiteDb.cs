using System;
using System.Collections.Generic;
using Db.Interfaces;

namespace Db.Models
{
    public class ParserSiteDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsChildParse { get; set; }
        public int IntervalFrom { get; set; }
        public int IntervalTo { get; set; }

        /// <summary>
        /// Notifications (key - value). Key - name notification, value - id(key) notification
        /// </summary>
        public string Notifications { get; set; }

        /// <summary>
        /// Include filter for data item (key - value). Key - name property item, value - include text
        /// </summary>
        public string IncludeFilter { get; set; }

        /// <summary>
        /// Exclude filter for data item (key - value). Key - name property item, value - include text
        /// </summary>
        public string ExcludeFilter { get; set; }

        public Guid ClientId { get; set; }
        public virtual ClientDb Client { get; set; }

        public Guid SiteId { get; set; }
        public virtual SiteDb Site { get; set; }

        public virtual ICollection<ItemDb> Items { get; set; }
    }
}