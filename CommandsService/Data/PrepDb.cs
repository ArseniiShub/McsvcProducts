using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public class PrepDb
{
	private readonly ILogger<PrepDb> _logger;

	public PrepDb(ILogger<PrepDb> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public void PrepPopulation(IApplicationBuilder app)
	{
		using var serviceScope = app.ApplicationServices.CreateScope();
		var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>()
		                 ?? throw new InvalidOperationException("Could not get IPlatformDataClient service");
		var platforms = grpcClient.ReturnAllPlatforms();

		var repository = serviceScope.ServiceProvider.GetService<ICommandRepo>()
		                 ?? throw new InvalidOperationException("Could not get ICommandRepo service");

		SeedData(repository, platforms);
	}

	private void SeedData(ICommandRepo repository, IEnumerable<Platform> platforms)
	{
		_logger.LogInformation("Seeding new platforms");

		foreach(var platform in platforms)
		{
			if(!repository.PlatformExists(platform.Id))
			{
				repository.CreatePlatform(platform);
			}

			repository.SaveChanges();
		}
	}
}
