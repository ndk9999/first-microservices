using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            CreateMap<Platform, PlatformRecord>();
            CreateMap<Command, CommandRecord>();
            CreateMap<CommandCreateModel, Command>();
            CreateMap<PlatformPublishedMessage, Platform>()
                .ForMember(p => p.ExternalId, opt => opt.MapFrom(m => m.Id))
                .ForMember(p => p.Id, opt => opt.Ignore());
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(p => p.ExternalId, opt => opt.MapFrom(m => m.PlatformId))
                .ForMember(p => p.Name, opt => opt.MapFrom(m => m.Name));
        }
    }
}