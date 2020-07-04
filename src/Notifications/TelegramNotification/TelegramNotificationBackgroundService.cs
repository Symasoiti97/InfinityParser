using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TelegramNotification
{
    public class TelegramNotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<TelegramNotificationBackgroundService> _logger;
        private readonly IBusControl _busControl;

        public TelegramNotificationBackgroundService(
            ILogger<TelegramNotificationBackgroundService> logger,
            IBusControl busControl)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TelegramNotificationService running");

            _busControl.StartAsync(stoppingToken);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TelegramNotificationService stopping");
            
            return base.StopAsync(cancellationToken);
        }
    }
}