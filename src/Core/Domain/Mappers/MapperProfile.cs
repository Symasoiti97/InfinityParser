using AutoMapper;
using Domain.Models;
using Dto;

namespace Domain.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<SiteDb, SiteDto>();
        }
    }
}