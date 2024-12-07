
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OrganizationService.Services
{
    public class OrganizationCreationRabbitMqConsumer : BackgroundService
    {
        private string _queueName;
        private readonly string _routingKey = "KK.EVENT.ADMIN.organizations.SUCCESS.ORGANIZATION.CREATE"; // Replace with your routing key if needed

        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly OrganizationCreationRabbitMqSender _sender;

        public OrganizationCreationRabbitMqConsumer(IServiceProvider serviceProvider, IConfiguration configuration, OrganizationCreationRabbitMqSender sender)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _sender = sender;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            // Establish connection and create a channel
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare a queue (optional for pre-declared queues like amq.topic)
            _queueName = _channel.QueueDeclare().QueueName;

            // Bind to the topic exchange
            _channel.QueueBind(queue: _queueName,
                               exchange: "amq.topic",
                               routingKey: _routingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => StopRabbitMQ());

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process the message
                await HandleMessageAsync(message);
            };

            _channel.BasicConsume(queue: _queueName,
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        private Task HandleMessageAsync(string message)
        {
            // Add your message processing logic here
            Console.WriteLine($"Received message: {message}");
            using (var scope = _serviceProvider.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
                if (JObject.Parse(message).ContainsKey("representation"))
                {
                    var schemaName = organizationService.AddOrganization((string)JObject.Parse(message)["representation"]!);
                
                    _sender.SendMessage("create.organization", schemaName);
                }
            }
            return Task.CompletedTask;
        }

        private void StopRabbitMQ()
        {
            _channel?.Close();
            _connection?.Close();
        }

        public override void Dispose()
        {
            StopRabbitMQ();
            base.Dispose();
        }
    }
}
