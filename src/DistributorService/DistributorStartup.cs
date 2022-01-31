using System;
using System.Timers;
using AutoMapper;
using Db;
using Db.Provider;
using DistributorService.Consumers;
using DistributorService.Services.Adapter;
using DistributorService.Services.Cache;
using DistributorService.Services.Filter;
using Helper.Mappers;
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

            services.AddAutoMapper((provider, config) => { config.AddProfile(new MapperProfile()); }, new Type[] { }, ServiceLifetime.Transient);

            services.AddDbContext<InfinityParserDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres")));
            services.AddScoped<IDataProvider, DataProvider>();

            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IAdapterService, AdapterService>();

            services.AddTransient<IFilterService, FilterService>();
            services.AddTransient<Timer>();

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