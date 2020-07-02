using System;
using System.Collections.Generic;
using System.Linq;
using Dto;

namespace DistributorService.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly Dictionary<Guid, HashSet<ItemDto>> _dictionary;

        public CacheService()
        {
            _dictionary = new Dictionary<Guid, HashSet<ItemDto>>();
        }

        public IEnumerable<T> SaveAndGetUncached<T>(Guid siteId, IEnumerable<T> itemDbs) where T : ItemDto
        {
            var itemsHash = new HashSet<ItemDto>(itemDbs);

            if (_dictionary.ContainsKey(siteId))
            {
                itemsHash.ExceptWith(_dictionary[siteId]);
                _dictionary[siteId].UnionWith(itemsHash);
            }
            else
            {
                _dictionary.Add(siteId, itemsHash);
            }

            return itemsHash.Cast<T>().ToList();
        }
    }
}