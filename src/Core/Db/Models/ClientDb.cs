using System;
using System.Collections.Generic;
using Db.Interfaces;

namespace Db.Models
{
    public class ClientDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<ParserSiteDb> ParserSites { get; set; }
    }
}