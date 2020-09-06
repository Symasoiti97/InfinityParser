using System;

namespace Dto
{
    public class ParserSiteDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Type ItemType { get; set; }
    }
}