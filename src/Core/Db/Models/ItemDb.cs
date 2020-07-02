using System;
using Db.Interfaces;

namespace Db.Models
{
    public class ItemDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public string Url { get; set; }

        public DateTime CreateDate { get; set; }
        
        public Guid SiteId { get; set; }
        public virtual SiteDb Site { get; set; }
    }
}