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
		CreateMap<Platform, GrpcPlatformModel>()
			.ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));
	}
}
