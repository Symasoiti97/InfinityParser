using System;
using System.Collections.Generic;
using Dto;

namespace DistributorService.Services.Cache
{
    public interface ICacheService
    {
        IEnumerable<T> SaveAndGetUncached<T>(Guid siteId, IEnumerable<T> itemDbs) where T : ItemDto;
    }
}