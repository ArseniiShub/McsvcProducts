namespace PlatformsService.Data;

//TODO For testing. Remove later
public class PrepDb
{
	private readonly ILogger<PrepDb> _logger;

	public PrepDb(ILogger<PrepDb> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}
	public void PrepPopulations(IApplicationBuilder app)
	{
		using var serviceScope = app.ApplicationServices.CreateScope();
		using var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();

		if(appDbContext == null)
		{
			throw new InvalidOperationException("Unable to get AppDbContext service");
		}

		SeedData(appDbContext);
	}

	private void SeedData(AppDbContext context)
	{
		if(context.Platforms.Any())
		{
			_logger.LogInformation("Data is already there. Skipping data creation");
			return;
		}

		_logger.LogInformation("Seeding Data...");

		context.Platforms.AddRange(
			new Platform { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
			new Platform { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
			new Platform { Name = "Kubernetes", Publisher = "Clod Native Computing Foundation", Cost = "Free" }
		);
		context.SaveChanges();
	}
}
