using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformsService.Dtos;
using PlatformsService.SyncDataServices.Http;

namespace PlatformsService.Controllers;

[Route("api/p/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
	private readonly ICommandDataClient _commandDataClient;
	private readonly ILogger<PlatformsController> _logger;
	private readonly IMapper _mapper;
	private readonly IPlatformRepo _repository;

	public PlatformsController(ILogger<PlatformsController> logger, IPlatformRepo repository, IMapper mapper,
		ICommandDataClient commandDataClient)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_commandDataClient = commandDataClient ?? throw new ArgumentNullException(nameof(commandDataClient));
	}

	[HttpGet]
	public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
	{
		_logger.LogInformation(">--- Getting Platforms");

		var platforms = _repository.GetAllPlatforms();
		return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
	}

	[HttpGet("{id:int}")]
	public ActionResult<PlatformReadDto> GetPlatformById(int id)
	{
		_logger.LogInformation(">--- Getting Platform with id: {Id}", id);

		var platform = _repository.GetPlatformById(id);
		if(platform != null)
		{
			return Ok(_mapper.Map<PlatformReadDto>(platform));
		}

		return NotFound();
	}

	[HttpPost]
	public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
	{
		_logger.LogInformation(">--- Creating new Platform");

		var platform = _mapper.Map<Platform>(platformCreateDto);
		_repository.CreatePlatform(platform);
		_repository.SaveChanges();

		var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

		try
		{
			await _commandDataClient.SendPlatformToCommandAsync(platformReadDto);
		}
		catch(Exception e)
		{
			_logger.LogError(e, ">--- Could not send synchronously");
		}

		return CreatedAtAction(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
	}
}
