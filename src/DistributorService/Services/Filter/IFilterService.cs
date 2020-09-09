using System.Collections.Generic;
using Dto;

namespace DistributorService.Services.Filter
{
    public interface IFilterService
    {
        IEnumerable<T> Filter<T>(IEnumerable<T> items, Dictionary<string, string[]> includeFilter, Dictionary<string, string[]> excludeFilter) where T : ItemDto;
    }
}