using System;
using System.Collections.Generic;
using Dto;

namespace DistributorService.Services.Cache
{
    public interface ICacheService
    {
        IEnumerable<T> SaveAndGetUncachedItem<T>(Guid siteId, IEnumerable<T> items) where T : ItemDto;
        public DataSiteDto GetCachedSiteOrNull(Guid siteId);
        public void SetCachedSite(DataSiteDto dataSite);
    }
}