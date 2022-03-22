using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Dtos;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
	private readonly ILogger<PlatformsController> _logger;
	private readonly IPlatformRepo _repository;
	private readonly IMapper _mapper;

	public PlatformsController(ILogger<PlatformsController> logger, IPlatformRepo repository, IMapper mapper)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
	public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
	{
		_logger.LogInformation(">--- Creating new Platform");

		var platform = _mapper.Map<Platform>(platformCreateDto);
		_repository.CreatePlatform(platform);
		_repository.SaveChanges();

		var platformReadDto = _mapper.Map<PlatformReadDto>(platform);
		return CreatedAtAction(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
	}
}
