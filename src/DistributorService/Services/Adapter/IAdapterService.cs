using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace DistributorService.Services.Adapter
{
    public interface IAdapterService
    {
        Task SaveAndPublishNotify<T>(ParserSiteDto parserSite, IEnumerable<T> items) where T : ItemDto;
    }
}