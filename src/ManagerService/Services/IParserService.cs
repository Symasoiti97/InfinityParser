using Dto;

namespace ManagerService.Services
{
    public interface IParserService
    {
        void StartParsing(SiteDto site);
        void RestartParsing();
        void StopParsing();
    }
}