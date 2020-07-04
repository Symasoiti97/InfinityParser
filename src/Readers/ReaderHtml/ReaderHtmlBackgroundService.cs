using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReaderHtml
{
    public class ReaderHtmlBackgroundService : BackgroundService
    {
        private readonly ILogger<ReaderHtmlBackgroundService> _logger;
        private readonly IBusControl _busControl;

        public ReaderHtmlBackgroundService(ILogger<ReaderHtmlBackgroundService> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReadHtmlService worker running");
            
            _busControl.StartAsync(stoppingToken);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReadHtmlService worker stopping");
            return base.StopAsync(cancellationToken);
        }
    }
}