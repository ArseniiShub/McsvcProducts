using System.Text;
using System.Text.Json;
using PlatformsService.Dtos;
using RabbitMQ.Client;

namespace PlatformsService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IDisposable
{
	private readonly ILogger<MessageBusClient> _logger;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private const string ExchangeName = "NewPlatformPublished";

	public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger)
	{
		ArgumentNullException.ThrowIfNull(configuration);
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		var factory = new ConnectionFactory
		{
			HostName = configuration["RabbitMQ:Host"],
			Port = int.Parse(configuration["RabbitMQ:Port"])
		};

		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
		_channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
		_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

		logger.LogInformation("Connected to MessageBus");
	}

	private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
	{
		_logger.LogInformation("RabbitMQ connection shutdown");
	}

	public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
	{
		var message = JsonSerializer.Serialize(platformPublishDto);

		if(_connection.IsOpen)
		{
			_logger.LogInformation("RabbitMQ connection is opened. Sending message...");
			SendMessage(message);
		}
		else
		{
			_logger.LogWarning("RabbitMQ connection is closed. Cant send message");
		}
	}

	private void SendMessage(string message)
	{
		var body = Encoding.UTF8.GetBytes(message);
		_channel.BasicPublish(ExchangeName, "", null, body);
		_logger.LogInformation("Message sent");
	}

	public void Dispose()
	{
		_logger.LogInformation("MessageBus disposed");

		if(_channel.IsOpen)
		{
			_channel.Close();
			_connection.Close();
		}
	}
}
