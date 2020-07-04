using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributorService
{
    public class DistributorBackgroundService : BackgroundService
    {
        private readonly ILogger<DistributorBackgroundService> _logger;
        private readonly IBusControl _busControl;

        public DistributorBackgroundService(ILogger<DistributorBackgroundService> logger, IBusControl busControl)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DistributorService running");

            _busControl.StartAsync(stoppingToken);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DistributorService stopping");
            return base.StopAsync(cancellationToken);
        }
    }
}