using AutoMapper;
using CommandsService.Dtos;

namespace CommandsService.Profiles;

public class PlatformsProfile : Profile
{
	public PlatformsProfile()
	{
		//Source => Target

		CreateMap<Platform, PlatformReadDto>();

		CreateMap<Command, CommandReadDto>();
		CreateMap<CommandCreateDto, Command>();
	}
}
