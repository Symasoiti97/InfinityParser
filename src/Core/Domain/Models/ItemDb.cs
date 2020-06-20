using System;

namespace Domain.Models
{
    public class ItemDb
    {
        public Guid Id { get; set; }
        public string Url { get; set; }

        public Guid SiteId { get; set; }
        public virtual SiteDb Site { get; set; }
    }
}