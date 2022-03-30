using System.Text.Json;
using AutoMapper;
using CommandsService.Dtos;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IMapper _mapper;
	private readonly ILogger<EventProcessor> _logger;

	public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<EventProcessor> logger)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public void ProcessEvent(string message)
	{
		var eventType = DetermineEventType(message);
		switch(eventType)
		{
			case EventType.PlatformPublished:
				AddPlatform(message);
				break;
		}
	}

	private EventType DetermineEventType(string notificationMessage)
	{
		_logger.LogInformation("Determining EventType");

		var genericEvent = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage)
		                   ?? throw new InvalidOperationException("Could not deserialize event type");
		switch(genericEvent.Event)
		{
			case "Platform_Published":
				_logger.LogInformation("PlatformPublished event detected");
				return EventType.PlatformPublished;
			default:
				_logger.LogInformation("Could not determine event type");
				return EventType.Undefined;
		}
	}

	private void AddPlatform(string platformPublishedMessage)
	{
		using var scope = _scopeFactory.CreateScope();
		var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

		var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);

		try
		{
			var plat = _mapper.Map<Platform>(platformPublishedDto);
			if(!repo.ExternalPlatformExists(plat.ExternalId))
			{
				repo.CreatePlatform(plat);
				repo.SaveChanges();
				_logger.LogInformation("Platform Added");
			}
			else
			{
				_logger.LogWarning("Platform already exists");
			}
		}
		catch(Exception e)
		{
			_logger.LogError(e, "Could not add platform to DB");
			throw;
		}
	}
}

public enum EventType
{
	PlatformPublished,
	Undefined
}
