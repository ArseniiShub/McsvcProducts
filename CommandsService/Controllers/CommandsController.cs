using AutoMapper;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
	private readonly ILogger<CommandsController> _logger;
	private readonly IMapper _mapper;
	private readonly ICommandRepo _repository;

	public CommandsController(ILogger<CommandsController> logger, ICommandRepo repository, IMapper mapper)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	}

	[HttpGet]
	public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
	{
		_logger.LogInformation("Getting all commands for platform with id: {PlatformId}", platformId);

		if(!_repository.PlatformExists(platformId))
		{
			return NotFound();
		}

		var commands = _repository.GetCommandsForPlatform(platformId);
		return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
	}

	[HttpGet("{commandId}")]
	public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
	{
		_logger.LogInformation("Getting command for platform with id: {PlatformId} and command with id: {CommandId}",
			platformId, commandId);

		if(!_repository.PlatformExists(platformId))
		{
			return NotFound();
		}

		var command = _repository.GetCommand(platformId, commandId);
		if(command == null)
		{
			return NotFound();
		}

		return Ok(_mapper.Map<CommandReadDto>(command));
	}

	[HttpPost]
	public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
	{
		_logger.LogInformation("Creating command for platform with id: {PlatformId}", platformId);

		if(!_repository.PlatformExists(platformId))
		{
			return NotFound();
		}

		var command = _mapper.Map<Command>(commandCreateDto);
		if(command == null)
		{
			return NotFound();
		}

		_repository.CreateCommand(platformId, command);
		_repository.SaveChanges();

		var commandReadDto = _mapper.Map<CommandReadDto>(command);

		return CreatedAtAction(
			nameof(GetCommandForPlatform),
			new { platformId, commandId = commandReadDto.Id },
			commandReadDto
		);
	}
}
