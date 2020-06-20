using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class SiteDb
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        
        public Guid ClientId { get; set; }
        public virtual ClientDb Client { get; set; }

        public virtual ICollection<ItemDb> Items { get; set; }
    }
}