using System;
using System.Threading.Tasks;
using Dto.HtmlMessage;
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
            _logger.LogInformation(context.Message.Url + " : " + context.Message.Type + " date: " + DateTime.Now, DateTimeOffset.Now);

            var htmlContent = _readerHtmlService.GetAsync(context.Message.Url).Result;

            _publishEndpoint.Publish(new HtmlMessageDto
            {
                HtmlContent = htmlContent
            });
            
            return Task.CompletedTask;
        }
    }
}