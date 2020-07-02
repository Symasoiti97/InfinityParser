using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace DistributorService.Services.Adapter
{
    public interface IAdapterService
    {
        Task SaveAndPublishNotify<T>(SiteDto site, IEnumerable<T> items) where T : ItemDto;
    }
}