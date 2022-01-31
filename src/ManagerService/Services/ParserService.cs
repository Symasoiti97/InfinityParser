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
        private DataSiteDto _dataSite;

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

        public void StartParsing(DataSiteDto dataSite)
        {
            _dataSite = dataSite ?? throw new ArgumentNullException(nameof(dataSite));

            SetTimerIntervalBySite();

            PublishMessage();

            _timer.Start();
        }

        public void RestartParsing()
        {
            if (_dataSite == null) throw new ArgumentNullException(nameof(_dataSite));

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
                ParserSite = new ParserSiteDto
                {
                    Id = _dataSite.Id,
                    Url = _dataSite.Url,
                    ItemType = _dataSite.ItemParentType
                }
            });

            _logger.LogInformation("ParserService: send message. Site: {0}", _dataSite);
        }

        private void SetTimerIntervalBySite()
        {
            var interval = _random.Next(_dataSite.IntervalFrom, _dataSite.IntervalTo);
            _timer.Interval = TimeSpan.FromSeconds(interval).TotalMilliseconds;
        }
    }
}