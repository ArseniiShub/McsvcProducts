using PlatformsService.Dtos;

namespace PlatformsService.AsyncDataServices;

public interface IMessageBusClient
{
	public void PublishNewPlatform(PlatformPublishDto platformPublishDto);
}
