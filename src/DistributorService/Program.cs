using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureLogging(loggerFactory => loggerFactory.AddEventLog())
                .ConfigureServices(DistributorStartup.ConfigureService);
    }
}