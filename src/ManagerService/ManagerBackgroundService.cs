using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ManagerService.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ManagerService
{
    public class ManagerBackgroundService : BackgroundService
    {
        private readonly ILogger<ManagerBackgroundService> _logger;
        private readonly IBusControl _busControl;
        private readonly IServiceProvider _serviceProvider;

        public ManagerBackgroundService(
            ILogger<ManagerBackgroundService> logger,
            IBusControl busControl,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Manager worker running at: {time}", DateTimeOffset.Now);
                
            _busControl.StartAsync(stoppingToken);
            
            var scope = _serviceProvider.CreateScope().ServiceProvider;
            var appStarter = scope.GetService<IApplicationsStarter>();
            appStarter.Start();
            
            return Task.CompletedTask;
        }
        
        private void StartServices()
        {
            var readerService = new Process {StartInfo = new ProcessStartInfo { }};
            readerService.Start();
            readerService.WaitForExit();

            var parserService = new Process {StartInfo = new ProcessStartInfo { }};
            readerService.Start();
            readerService.WaitForExit();

            var distributorService = new Process {StartInfo = new ProcessStartInfo { }};
            readerService.Start();
            readerService.WaitForExit();

            var notificationService = new Process {StartInfo = new ProcessStartInfo { }};
            readerService.Start();
            readerService.WaitForExit();
        }
    }
}