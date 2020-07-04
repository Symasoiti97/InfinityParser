using System;
using System.Timers;
using Dto;
using Dto.QueueMessages;
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
        }

        public void Init(SiteDto site)
        {
            _site = site;
            _timer.Elapsed += TimerOnElapsed;
            var interval = new Random().Next(_site.IntervalFrom, _site.IntervalTo);
            _timer.Interval = TimeSpan.FromSeconds(interval).TotalMilliseconds;
        }

        public void StartParsing()
        {
            if(_site == null) throw new ArgumentNullException(nameof(_site));
            
            PublishMessage();
            
            _timer.Start();
        }

        public void StopParsing()
        {
            _timer.Stop();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            PublishMessage();
        }

        private void PublishMessage()
        {
            _massTransitCenter.Publish(new SiteMessageDto
            {
                Site = _site
            });

            _logger.LogInformation("ParserService: send message. Date: {time}", DateTimeOffset.Now);
        }
    }
}