using AutoMapper;
using Grpc.Core;

namespace PlatformsService.SyncDataServices.Grpc;

public class GrpcPlatformsService : GrpcPlatform.GrpcPlatformBase
{
	private readonly IPlatformRepo _repository;
	private readonly IMapper _mapper;

	public GrpcPlatformsService(IPlatformRepo repository, IMapper mapper)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	}

	public override Task<PlatformsResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
	{
		var response = new PlatformsResponse();
		var platforms = _repository.GetAllPlatforms();
		response.Platforms.AddRange(_mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms));

		return Task.FromResult(response);
	}
}
