using Logger;
using Microsoft.Extensions.Hosting;

namespace ReaderHtml
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
                .ConfigureLogging(loggerFactory => loggerFactory.AddFileProvider())
                .ConfigureServices(ReaderHtmlStartup.ConfigureService);
    }
}