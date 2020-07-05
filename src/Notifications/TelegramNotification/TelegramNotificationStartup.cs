using System;
using System.Net.Http;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Queue;
using Telegram.Bot;
using TelegramNotification.Consumers;
using TelegramNotification.Options;
using TelegramNotification.Services;

namespace TelegramNotification
{
    public static class TelegramNotificationStartup
    {
        public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (services == null) throw new ArgumentNullException(nameof(services));

            var configuration = context.Configuration;

            ConfigureOptions(services, configuration);

            ConfigureMassTransit(services);

            services.AddTransient<ITelegramBotClient, TelegramBotClient>(provider =>
            {
                var tokenBot = provider.GetService<IOptions<TelegramApiOptions>>().Value;

                return new TelegramBotClient(tokenBot.TokenBot, new HttpClient());
            });
            services.AddTransient<ITelegramService, TelegramService>();

            services.AddHostedService<TelegramNotificationBackgroundService>();
        }

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
            services.Configure<TelegramApiOptions>(configuration.GetSection(nameof(TelegramApiOptions)));
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.RegisterConsumers();

                cfg.RegisterBus((provider, register) =>
                {
                    register.ReceiveEndpoint("telegram-notify", regCfg =>
                    {
                        var reg = provider.GetService<IRegistration>();
                        regCfg.ConfigureConsumer(reg, typeof(ThreeNineMdTelegramNotificationConsumer));
                    });
                });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }

        private static void RegisterConsumers(this IServiceCollectionConfigurator servicesConfigurator)
        {
            servicesConfigurator.AddConsumer<ThreeNineMdTelegramNotificationConsumer>();
        }
    }
}