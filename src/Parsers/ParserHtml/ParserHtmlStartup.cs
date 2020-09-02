using System;
using System.Collections.Generic;
using Dto.ThreeNineMd;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParserHtml.Consumers;
using ParserHtml.Services;
using ParserHtml.Services.ThreeNineMd;
using Queue;

namespace ParserHtml
{
    public static class ParserHtmlStartup
    {
        public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (services == null) throw new ArgumentNullException(nameof(services));

            var configuration = context.Configuration;

            ConfigureOptions(services, configuration);

            ConfigureMassTransit(services);

            services.AddTransient<IParserService<IEnumerable<ShortThreeNineMdItem>>, ThreeNineMdParser>();

            services.AddHostedService<ParserHtmlBackgroundService>();
        }

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<ParserHtmlConsumer>();

                cfg.RegisterBus((provider, register) => { register.ReceiveEndpoint<ParserHtmlConsumer>(provider); });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }
    }
}