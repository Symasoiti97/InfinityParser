﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Db.Models;
using Domain.Provider;
using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerService.Services
{
    public class ApplicationStarter : IApplicationsStarter
    {
        private readonly IDataProvider _dataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public ApplicationStarter(
            IDataProvider dataProvider,
            IServiceProvider serviceProvider,
            IMapper mapper)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Start()
        {
            var sites = await _dataProvider.Get<SiteDb>().ProjectTo<SiteDto>(_mapper.ConfigurationProvider).ToArrayAsync();
            
            var scope = _serviceProvider.CreateScope();
            foreach (var site in sites)
            {
                var parser = scope.ServiceProvider.GetService<IParserService>();
                parser.StartParsing(site);
            }
        }
    }
}