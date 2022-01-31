using System;
using System.Collections.Generic;
using Db.Interfaces;

namespace Db.Models
{
    public class UserDb : IEntity<Guid>, ICreatable
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<ParserSiteDb> ParserSites { get; set; }
    }
}