using System;
using DistributorService.Consumers;
using DistributorService.Services.Adapter;
using DistributorService.Services.Cache;
using Domain;
using Domain.Provider;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
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
            
            services.AddDbContext<InfinityParserDbContext>(optionsBuilder =>
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
                cfg.RegisterConsumers();

                cfg.RegisterBus((provider, register) =>
                {
                    register.ReceiveEndpoint("distributor", regCfg =>
                    {
                        var reg = provider.GetService<IRegistration>();
                        regCfg.ConfigureConsumer(reg, typeof(DistributorConsumer));
                    });
                });
            });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }

        private static void RegisterConsumers(this IServiceCollectionConfigurator servicesConfigurator)
        {
            servicesConfigurator.AddConsumer<DistributorConsumer>();
        }
    }
}