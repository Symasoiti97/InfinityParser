using System;
using System.Threading.Tasks;
using Dto.QueueMessages;
using MassTransit;
using Microsoft.Extensions.Logging;
using ReaderHtml.Services;

namespace ReaderHtml.Consumers
{
    public class ReaderHtmlConsumer : IConsumer<SiteMessageDto>
    {
        private readonly ILogger<ReaderHtmlConsumer> _logger;
        private readonly IReaderHtmlService _readerHtmlService;
        private readonly IPublishEndpoint _publishEndpoint;

        public ReaderHtmlConsumer(
            ILogger<ReaderHtmlConsumer> logger,
            IReaderHtmlService readerHtmlService,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _readerHtmlService = readerHtmlService ?? throw new ArgumentNullException(nameof(readerHtmlService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Consume(ConsumeContext<SiteMessageDto> context)
        {
            var message = context.Message;

            _logger.LogInformation("{0} : Url: {1} | ItemType: {2}", typeof(ReaderHtmlConsumer).Name, message.ParserSite.Url, message.ParserSite.ItemType);

            var htmlContent = _readerHtmlService.GetAsync(message.ParserSite.Url).Result;

            await _publishEndpoint.Publish(new HtmlMessageDto
            {
                ParserSite = message.ParserSite,
                HtmlContent = htmlContent
            });
        }
    }
}