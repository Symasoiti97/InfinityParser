using Dto;

namespace ManagerService.Services
{
    public interface IParserService
    {
        void Init(SiteDto site);
        void StartParsing();
        void StopParsing();
    }
}