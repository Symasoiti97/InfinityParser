using System;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Queue
{
    public static class MassTransitConfigurator
    {
        public static void RegisterBus(this IServiceCollectionConfigurator servicesConfigurator, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> register)
        {
            servicesConfigurator.AddBus(context => CreateRabbitMqBusControl(context.Container, register));
        }

        private static IBusControl CreateRabbitMqBusControl(IServiceProvider provider, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> register)
        {
            var rabbitMqOptions = provider.GetService<IOptions<RabbitMqOptions>>().Value;
            var busControl = Bus.Factory.CreateUsingRabbitMq(configurator =>
            {
                var uri = new UriBuilder("rabbitmq", rabbitMqOptions.Host, rabbitMqOptions.Port).Uri;
                configurator.Host(uri, hostConfigurator => { });

                //configurator.AutoDelete = false;
                //configurator.Durable = true;
                
                register(provider, configurator);
            });

            return busControl;
        }
    }
}