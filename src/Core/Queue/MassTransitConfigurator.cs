using System;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Options;

namespace Queue
{
    public static class MassTransitConfigurator
    {
        public static void RegisterBus(this IServiceCollectionBusConfigurator servicesConfigurator, Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> register = null)
        {
            servicesConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = context.GetService<IOptions<RabbitMqOptions>>().Value;
                var uri = new UriBuilder(rabbitMqOptions.Scheme, rabbitMqOptions.Host, rabbitMqOptions.Port).Uri;
                cfg.Host(uri);

                register?.Invoke(context, cfg);
            });
        }

        public static void ReceiveEndpoint<TConsumer>(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            var queueName = BuildQueueName<TConsumer>();
            configurator.ReceiveEndpoint(queueName, e =>
            {
                e.ConfigureConsumer(context, typeof(TConsumer));
                
                configure?.Invoke(e);
            });
        }

        private static string BuildQueueName<TConsumer>()
        {
            var typeConsumer = typeof(TConsumer);
            return typeConsumer.Name.ToLower().Replace("consumer", "");
        }
    }
}