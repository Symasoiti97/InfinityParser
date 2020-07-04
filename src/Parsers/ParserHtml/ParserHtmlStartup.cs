using System;
using System.Collections.Generic;
using Dto.ThreeNineMd;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
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
                cfg.RegisterConsumers();

                cfg.RegisterBus((provider, register) =>
                {
                    register.ReceiveEndpoint("parser-html", regCfg =>
                    {
                        var reg = provider.GetService<IRegistration>();
                        regCfg.ConfigureConsumer(reg, typeof(ParserHtmlConsumer));
                    });
                });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }

        private static void RegisterConsumers(this IServiceCollectionConfigurator servicesConfigurator)
        {
            servicesConfigurator.AddConsumer<ParserHtmlConsumer>();
        }
    }
}