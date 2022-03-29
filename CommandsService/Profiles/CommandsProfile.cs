using AutoMapper;
using CommandsService.Dtos;

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
	}
}
