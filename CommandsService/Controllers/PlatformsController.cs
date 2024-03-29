﻿using AutoMapper;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
	private readonly ILogger<PlatformsController> _logger;
	private readonly IMapper _mapper;
	private readonly ICommandRepo _repository;

	public PlatformsController(ILogger<PlatformsController> logger, ICommandRepo repository, IMapper mapper)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	}

	[HttpGet]
	public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
	{
		_logger.LogInformation("Getting all platforms from CommandsService");

		var platforms = _repository.GetAllPlatforms();
		return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
	}

	[HttpPost]
	public ActionResult TestInboundConnection()
	{
		Console.WriteLine("--> Inbound POST # Command Service");

		return Ok("Inbound test of from Platforms Service");
	}
}
