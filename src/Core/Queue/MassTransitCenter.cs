using System;
using System.Threading.Tasks;
using MassTransit;

namespace Queue
{
    public class MassTransitCenter : IMassTransitCenter
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitCenter(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        
        public async Task Publish<T>(T message) where T : class
        {
            await _publishEndpoint.Publish(message);
        }
    }
}