﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dto.QueueMessages;
using Dto.ThreeNineMd;
using Helper.Extensions;
using MassTransit;
using Microsoft.Extensions.Logging;
using ParserHtml.Services;

namespace ParserHtml.Consumers
{
    public class ParserHtmlConsumer : IConsumer<HtmlMessageDto>
    {
        private readonly ILogger<ParserHtmlConsumer> _logger;
        private readonly IParserService<IEnumerable<ShortThreeNineMdItem>> _parserService;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParserHtmlConsumer(
            ILogger<ParserHtmlConsumer> logger,
            IParserService<IEnumerable<ShortThreeNineMdItem>> parserService,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public Task Consume(ConsumeContext<HtmlMessageDto> context)
        {
            _logger.LogInformation("ParserHtml. Date: {0}", DateTimeOffset.Now);

            var items = _parserService.ParseHtmlContent(context.Message.HtmlContent).Result;

            _publishEndpoint.Publish(new DistributorMessageDto
            {
                ThreeNineMdCollection = items,
                Site = context.Message.Site
            });

            return Task.CompletedTask;
        }
    }
}