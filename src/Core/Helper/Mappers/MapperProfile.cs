using System;
using System.Collections.Generic;
using AutoMapper;
using Db.Models;
using Dto;
using Dto.Common;
using Helper.Extensions;

namespace Helper.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ParserSiteDb, DataSiteDto>()
                .ForMember(d => d.Url, o => o.MapFrom(s => s.Site.Url))
                .ForMember(d => d.ExcludeFilter, o => o.MapFrom(s => MapFromJson<Dictionary<string, string[]>>(s.ExcludeFilter)))
                .ForMember(d => d.IncludeFilter, o => o.MapFrom(s => MapFromJson<Dictionary<string, string[]>>(s.IncludeFilter)))
                .ForMember(d => d.Notifications, o => o.MapFrom(s => MapFromJson<Dictionary<NotificationType, string>>(s.Notifications)))
                .ForMember(d => d.ItemType, o => o.MapFrom(s => s.Site.ItemType))
                .ForMember(d => d.ItemChildType, o => o.MapFrom(s => s.Site.ItemChildType != null ? Type.GetType(s.Site.ItemChildType) : null))
                .ForMember(d => d.ItemParentType, o => o.MapFrom(s => Type.GetType(s.Site.ItemParentType)));

            CreateMap<ParserSiteDb, ParserSiteDto>()
                .ForMember(d => d.Url, o => o.MapFrom(s => s.Site.Url))
                .ForMember(d => d.ItemType, o => o.MapFrom(s => s.Site.ItemParentType));

            CreateMap<DataSiteDto, DataSiteDto>();

            CreateMap<ClientDb, ClientDto>();
        }

        private static T MapFromJson<T>(string value) where T : new()
        {
            return string.IsNullOrWhiteSpace(value) ? new T() : value.FromJson<T>();
        }
    }
}