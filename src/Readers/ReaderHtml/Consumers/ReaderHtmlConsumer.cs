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

        public Task Consume(ConsumeContext<SiteMessageDto> context)
        {
            var site = context.Message.Site;
            
            _logger.LogInformation("{0} : {1} | Date: {2}", site.Url, site.ItemType, DateTimeOffset.Now);

            var htmlContent = _readerHtmlService.GetAsync(site.Url).Result;

            _publishEndpoint.Publish(new HtmlMessageDto
            {
                Site = site,
                HtmlContent = htmlContent
            });
            
            return Task.CompletedTask;
        }
    }
}