using AutoMapper;
using Grpc.Net.Client;
using PlatformsService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
	private readonly IConfiguration _configuration;
	private readonly IMapper _mapper;
	private readonly ILogger<PlatformDataClient> _logger;

	public PlatformDataClient(IConfiguration configuration, IMapper mapper, ILogger<PlatformDataClient> logger)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public IEnumerable<Platform> ReturnAllPlatforms()
	{
		var url = _configuration["GrpcPlatformsUrl"];
		_logger.LogInformation("Calling Grpc Service {Url}", url);

		var channel = GrpcChannel.ForAddress(url);
		var client = new GrpcPlatform.GrpcPlatformClient(channel);
		var request = new GetAllRequest();

		try
		{
			var reply = client.GetAllPlatforms(request);
			return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
		}
		catch(Exception e)
		{
			_logger.LogError(e, "Could not call Grpc server");
			throw;
		}
	}
}
