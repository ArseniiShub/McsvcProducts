using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<MessageBusSubscriber> _logger;
	private readonly IEventProcessor _eventProcessor;
	private IConnection _connection = null!;
	private IModel _channel = null!;
	private string _queueName = "";

	private const string ExchangeName = "NewPlatformPublished";

	public MessageBusSubscriber(IConfiguration configuration, ILogger<MessageBusSubscriber> logger,
		IEventProcessor eventProcessor)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));

		InitializeRabbitMQ();
	}

	private void InitializeRabbitMQ()
	{
		var factory = new ConnectionFactory
		{
			HostName = _configuration["RabbitMQ:Host"],
			Port = int.Parse(_configuration["RabbitMQ:Port"])
		};
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
		_channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
		_queueName = _channel.QueueDeclare().QueueName;
		_channel.QueueBind(_queueName, ExchangeName, "");

		_logger.LogInformation("Listening on MessageBus");

		_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
	}

	private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
	{
		_logger.LogInformation("RabbitMQ connection shutdown");
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.ThrowIfCancellationRequested();

		var consumer = new EventingBasicConsumer(_channel);
		consumer.Received += OnEventReceived;
		_channel.BasicConsume(_queueName, true, consumer);

		return Task.CompletedTask;
	}

	private void OnEventReceived(object? sender, BasicDeliverEventArgs e)
	{
		_logger.LogInformation("Event received");

		var body = e.Body;
		var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

		_eventProcessor.ProcessEvent(notificationMessage);
	}

	public override void Dispose()
	{
		if(_channel.IsOpen)
		{
			_channel.Close();
			_connection.Close();
		}

		base.Dispose();
	}
}
