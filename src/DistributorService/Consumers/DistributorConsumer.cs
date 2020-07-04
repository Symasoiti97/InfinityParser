using System;
using System.Threading.Tasks;
using DistributorService.Services.Adapter;
using Dto.QueueMessages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DistributorService.Consumers
{
    public class DistributorConsumer : IConsumer<DistributorMessageDto>
    {
        private readonly ILogger<DistributorConsumer> _logger;
        private readonly IAdapterService _adapterService;

        public DistributorConsumer(
            ILogger<DistributorConsumer> logger,
            IAdapterService adapterService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _adapterService = adapterService ?? throw new ArgumentNullException(nameof(adapterService));
        }

        public Task Consume(ConsumeContext<DistributorMessageDto> context)
        {
            _logger.LogInformation("[{0}]:\t{1} - Get Message", DateTimeOffset.Now, typeof(DistributorConsumer).Name);

            _adapterService.SaveAndPublishNotify(context.Message.Site, context.Message.ThreeNineMdCollection).GetAwaiter();

            return Task.CompletedTask;
        }
    }
}