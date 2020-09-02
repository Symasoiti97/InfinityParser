using System;
using DistributorService.Consumers;
using DistributorService.Services.Adapter;
using DistributorService.Services.Cache;
using Domain;
using Domain.Provider;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Queue;

namespace DistributorService
{
    public static class DistributorStartup
    {
        public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (services == null) throw new ArgumentNullException(nameof(services));

            var configuration = context.Configuration;

            ConfigureOptions(services, configuration);

            ConfigureMassTransit(services);

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<InfinityParserDbContext>(optionsBuilder =>
                    optionsBuilder.UseNpgsql(configuration["ConnectionString:Postgres"]));
            services.AddScoped<IDataProvider, DataProvider>();

            services.AddSingleton<ICacheService, CacheService>();
            services.AddTransient<IAdapterService, AdapterService>();

            services.AddHostedService<DistributorBackgroundService>();
        }

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<DistributorConsumer>();

                cfg.RegisterBus((provider, register) => { register.ReceiveEndpoint<DistributorConsumer>(provider); });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }
    }
}