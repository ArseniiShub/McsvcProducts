using PlatformsService.Dtos;

namespace PlatformsService.SyncDataServices.Http;

public interface ICommandDataClient
{
	Task SendPlatformToCommandAsync(PlatformReadDto platform);
}
