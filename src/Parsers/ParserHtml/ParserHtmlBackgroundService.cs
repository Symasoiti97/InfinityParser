using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ParserHtml
{
    public class ParserHtmlBackgroundService : BackgroundService
    {
        private readonly ILogger<ParserHtmlBackgroundService> _logger;
        private readonly IBusControl _busControl;

        public ParserHtmlBackgroundService(ILogger<ParserHtmlBackgroundService> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ParserHtmlService worker running at: {time}", DateTimeOffset.Now);
            
            _busControl.StartAsync(stoppingToken);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ParserHtmlService stopping at: {time}", DateTimeOffset.Now);
            
            return base.StopAsync(cancellationToken);
        }
    }
}