using System;
using Db.Interfaces;

namespace Db.Models
{
    public class ItemDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public string Url { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid ParserSiteId { get; set; }
        public virtual ParserSiteDb ParserSite { get; set; }
    }
}