using AutoMapper;
using PlatformsService.Dtos;

namespace PlatformsService.Profiles;

public class PlatformsProfile : Profile
{
	public PlatformsProfile()
	{
		//Source => Target
		CreateMap<Platform, PlatformReadDto>();
		CreateMap<PlatformCreateDto, Platform>();
		CreateMap<PlatformReadDto, PlatformPublishDto>();
	}
}
