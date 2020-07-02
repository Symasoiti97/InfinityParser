using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dto.Common;
using Helper.Extensions;
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
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            _busControl.StartAsync(stoppingToken);

            return Task.CompletedTask;
        }
    }
}