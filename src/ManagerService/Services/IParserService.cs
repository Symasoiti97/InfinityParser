using Dto;

namespace ManagerService.Services
{
    public interface IParserService
    {
        void StartParsing(DataSiteDto dataSite);
        void RestartParsing();
        void StopParsing();
    }
}