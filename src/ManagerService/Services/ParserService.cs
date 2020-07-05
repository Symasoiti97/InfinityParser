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
        private readonly Random _random;
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
            _random = new Random();
        }
        
        public void StartParsing(SiteDto site)
        {
            _site = site ?? throw new ArgumentNullException(nameof(_site));

            SetTimerIntervalBySite();
            
            PublishMessage();
            
            _timer.Start();
        }
        
        public void RestartParsing()
        {
            if(_site == null) throw new ArgumentNullException(nameof(_site));
            
            SetTimerIntervalBySite();
            
            _timer.Start();
        }

        public void StopParsing()
        {
            _timer.Stop();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            StopParsing();
            
            PublishMessage();

            RestartParsing();
        }

        private void PublishMessage()
        {
            _massTransitCenter.Publish(new SiteMessageDto
            {
                Site = _site
            });

            _logger.LogInformation("ParserService: send message. Site: {0}", _site);
        }

        private void SetTimerIntervalBySite()
        {
            var interval = _random.Next(_site.IntervalFrom, _site.IntervalTo);
            _timer.Interval = TimeSpan.FromSeconds(interval).TotalMilliseconds;
        }
    }
}