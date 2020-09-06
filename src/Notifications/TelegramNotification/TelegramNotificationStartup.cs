using System;
using System.Net.Http;
using GreenPipes;
using MassTransit;
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
                cfg.AddConsumer<TelegramNotificationConsumer>();

                cfg.RegisterBus((provider, register) => { register.ReceiveEndpoint<TelegramNotificationConsumer>(provider, configurator => configurator.UseRateLimit(20, TimeSpan.FromMinutes(1))); });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }
    }
}