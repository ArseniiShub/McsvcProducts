using AutoMapper;
using CommandsService.Dtos;
using PlatformsService;

namespace CommandsService.Profiles;

public class CommandsProfile : Profile
{
	public CommandsProfile()
	{
		//Source => Target

		CreateMap<Command, CommandReadDto>();
		CreateMap<CommandCreateDto, Command>();

		CreateMap<Platform, PlatformReadDto>();
		CreateMap<PlatformPublishDto, Platform>()
			.ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
		CreateMap<GrpcPlatformModel, Platform>()
			.ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Commands, opt => opt.Ignore());
	}
}
