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
            CreateMap<SiteDb, SiteDto>()
                .ForMember(d => d.Notifications, o => o.MapFrom(s => s.Notifications.FromJson<Dictionary<NotificationType, string>>()));

            CreateMap<ClientDb, ClientDto>();
        }
    }
}