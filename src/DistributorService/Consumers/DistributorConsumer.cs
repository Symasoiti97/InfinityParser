using System;
using System.Threading.Tasks;
using DistributorService.Services.Adapter;
using Dto.QueueMessages;
using Helper.Extensions;
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

        public async Task Consume(ConsumeContext<DistributorMessageDto> context)
        {
            _logger.LogInformation("{0} - Get Message", nameof(DistributorConsumer));

            var message = context.Message;
            var items = message.Items.ToObject(message.ParserSite.ItemType.ToEnumerableType());

            await _adapterService.SaveAndPublishNotify(message.ParserSite, items);
        }
    }
}