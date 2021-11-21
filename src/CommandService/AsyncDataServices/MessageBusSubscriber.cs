using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IEventProcessor eventProcessor, IConfiguration configuration)
        {
            _eventProcessor = eventProcessor;
            _configuration = configuration;

            InitializeRabbitMQ();
        }

        public override void Dispose()
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (moduleHandle, eventArgs) => 
            {
                Console.WriteLine("--> Event Received!");

                var body = eventArgs.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                 _connection = factory.CreateConnection();
                 _channel = _connection.CreateModel();
                 _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                 _queueName = _channel.QueueDeclare().QueueName;
                 _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

                 Console.WriteLine("--> Listening on the Message Bus...");

                 _connection.ConnectionBlocked += RabbitMQ_ConnectionShutdown;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Could not connect to RabbitMQ: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ConnectionBlockedEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}