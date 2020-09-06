using System;
using System.Collections.Generic;
using Db.Interfaces;
using Db.Models.Common;

namespace Db.Models
{
    public class SiteDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public string BaseUrl { get; set; }
        public string Url { get; set; }
        public ItemType ItemType { get; set; }

        /// <summary>
        /// This type for GetType()
        /// </summary>
        public string ItemParentType { get; set; }

        /// <summary>
        /// This type for GetType()
        /// </summary>
        public string ItemChildType { get; set; }

        public virtual ICollection<ParserSiteDb> ParserSites { get; set; }
    }
}