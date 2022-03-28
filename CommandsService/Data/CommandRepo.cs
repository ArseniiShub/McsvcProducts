﻿namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
	private readonly AppDbContext _context;

	public CommandRepo(AppDbContext context)
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

	public void CreatePlatform(Platform platform)
	{
		ArgumentNullException.ThrowIfNull(platform);

		_context.Platforms.Add(platform);
	}

	public bool PlatformExists(int platformId)
	{
		return _context.Platforms.Any(p => p.Id == platformId);
	}

	public IEnumerable<Command> GetCommandsForPlatform(int platformId)
	{
		return _context.Commands
			.Where(c => c.PlatformId == platformId)
			.OrderBy(c => c.Platform.Name);
	}

	public Command? GetCommand(int platformId, int commandId)
	{
		return _context.Commands.FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
	}

	public void CreateCommand(int platformId, Command command)
	{
		ArgumentNullException.ThrowIfNull(command);

		command.PlatformId = platformId;
		_context.Commands.Add(command);
	}
}