using System;
using System.Timers;
using Domain;
using Domain.Provider;
using ManagerService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Queue;
using AutoMapper;
using Helper.Mappers;

namespace ManagerService
{
    public class ManagerStartup
    {
        public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHostedService<ManagerBackgroundService>();

            var configuration = context.Configuration;
            ConfigureOptions(services, configuration);
            ConfigureMassTransit(services);

            services.AddAutoMapper((provider, config) => { config.AddProfile(new MapperProfile()); }, new Type[] { }, ServiceLifetime.Transient);

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<InfinityParserDbContext>(optionsBuilder =>
                    optionsBuilder.UseNpgsql(
                        "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=InfinityParser;Pooling=true;MinPoolSize=15;MaxPoolSize=20;CommandTimeout=20;Timeout=20"));
            services.AddScoped<IDataProvider, DataProvider>();

            services.AddTransient<Timer>();
            services.AddTransient<IParserService, ParserService>();
            services.AddTransient<IApplicationsStarter, ApplicationStarter>();
        }

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(cfg => { cfg.RegisterBus((provider, register) => { }); });

            services.AddScoped<IMassTransitCenter, MassTransitCenter>();
        }
    }
}