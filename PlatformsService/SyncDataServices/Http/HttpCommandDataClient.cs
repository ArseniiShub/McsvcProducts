using System.Text;
using System.Text.Json;
using PlatformsService.Dtos;

namespace PlatformsService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
	private readonly IConfiguration _configuration;
	private readonly HttpClient _httpClient;
	private readonly ILogger<HttpCommandDataClient> _logger;

	public HttpCommandDataClient(ILogger<HttpCommandDataClient> logger, HttpClient httpClient,
		IConfiguration configuration)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	}

	public async Task SendPlatformToCommandAsync(PlatformReadDto platform)
	{
		var httpContent = new StringContent(
			JsonSerializer.Serialize(platform),
			Encoding.UTF8,
			"application/json"
		);

		var response = await _httpClient.PostAsync(_configuration["CommandsService"], httpContent);

		if(response.IsSuccessStatusCode)
		{
			_logger.LogInformation(">--- Sync POST to CommandsService was OK!");
		}
		else
		{
			_logger.LogInformation(">--- Sync POST to CommandsService was NOT OK!");
		}
	}
}
