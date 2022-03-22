namespace PlatformService.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Platform> Platforms { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Platform>()
			.HasKey(p => p.Id);
	}
}
