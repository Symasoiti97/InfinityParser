using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class ClientDb
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SiteDb> Sites { get; set; }
    }
}