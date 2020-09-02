using System;
using System.Net.Http;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Queue;
using ReaderHtml.Consumers;
using ReaderHtml.Services;

namespace ReaderHtml
{
    public static class ReaderHtmlStartup
    {
        public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (services == null) throw new ArgumentNullException(nameof(services));

            var configuration = context.Configuration;

            ConfigureOptions(services, configuration);

            ConfigureMassTransit(services);

            services.AddTransient<HttpClient>();
            services.AddTransient<IReaderHtmlService, ReaderWebClientService>();

            services.AddHostedService<ReaderHtmlBackgroundService>();
        }

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<ReaderHtmlConsumer>();

                cfg.RegisterBus((provider, register) => { register.ReceiveEndpoint<ReaderHtmlConsumer>(provider); });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }
    }
}