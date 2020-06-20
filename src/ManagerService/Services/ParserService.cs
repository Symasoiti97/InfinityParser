using System;
using System.Timers;
using Dto;
using Dto.HtmlMessage;
using Microsoft.Extensions.Logging;
using Queue;

namespace ManagerService.Services
{
    public class ParserService : IParserService
    {
        private readonly Timer _timer;
        private readonly IMassTransitCenter _massTransitCenter;
        private readonly ILogger<ParserService> _logger;
        private SiteDto _site;

        public ParserService(
            Timer timer,
            IMassTransitCenter massTransitCenter,
            ILogger<ParserService> logger)
        {
            _massTransitCenter = massTransitCenter ?? throw new ArgumentNullException(nameof(massTransitCenter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timer = timer ?? throw new ArgumentNullException(nameof(timer));
            _timer.Elapsed += TimerOnElapsed;
            _timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
        }

        public void Init(SiteDto site)
        {
            _site = site;
        }

        public void StartParsing()
        {
            if(_site == null) throw new ArgumentNullException(nameof(_site));
            
            _timer.Start();
        }

        public void StopParsing()
        {
            _timer.Stop();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _massTransitCenter.Publish(new SiteMessageDto
            {
                Url = _site.Url,
                Type = _site.Type
            });

            _logger.LogInformation($"Send message. Date: {DateTime.Now}", DateTimeOffset.Now);
        }
    }
}