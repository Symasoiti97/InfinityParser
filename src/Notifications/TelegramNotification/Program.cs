using Microsoft.Extensions.Hosting;

namespace TelegramNotification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(TelegramNotificationStartup.ConfigureService);
    }
}