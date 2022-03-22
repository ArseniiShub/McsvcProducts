namespace PlatformService.Data;

public class PlatformRepo : IPlatformRepo
{
	private readonly AppDbContext _context;

	public PlatformRepo(AppDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public bool SaveChanges()
	{
		return _context.SaveChanges() >= 0;
	}

	public IEnumerable<Platform> GetAllPlatforms()
	{
		return _context.Platforms.ToList();
	}

	public Platform? GetPlatformById(int id)
	{
		return _context.Platforms.FirstOrDefault(x => x.Id == id);
	}

	public void CreatePlatform(Platform platform)
	{
		ArgumentNullException.ThrowIfNull(platform);

		_context.Platforms.Add(platform);
	}
}
