using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformRecord>();
            CreateMap<PlatformCreateModel, Platform>();
            CreateMap<PlatformRecord, PlatformPublishedMessage>();
            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(p => p.PlatformId, opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(p => p.Publisher, opt => opt.MapFrom(s => s.Publisher));
        }
    }
}