using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dto.QueueMessages;
using MassTransit;
using Microsoft.Extensions.Logging;
using ParserHtml.Services;

namespace ParserHtml.Consumers
{
    public class ParserHtmlConsumer : IConsumer<HtmlMessageDto>
    {
        private readonly ILogger<ParserHtmlConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParserHtmlConsumer(
            ILogger<ParserHtmlConsumer> logger,
            IServiceProvider serviceProvider,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public Task Consume(ConsumeContext<HtmlMessageDto> context)
        {
            _logger.LogInformation("{0} - Get Message", typeof(ParserHtmlConsumer).Name);

            var parserService = GetParserHtmlService(context.Message.Site.ItemClass);
            var items = parserService.ParseHtmlContent(context.Message.HtmlContent).Result;

            _publishEndpoint.Publish(new DistributorMessageDto
            {
                Items = items,
                Site = context.Message.Site
            });

            return Task.CompletedTask;
        }

        private dynamic GetParserHtmlService(Type itemType)
        {
            var iEnumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
            var parserHtmlServiceType = typeof(IParserService<>).MakeGenericType(iEnumerableType);

            return _serviceProvider.GetService(parserHtmlServiceType);
        }
    }
}