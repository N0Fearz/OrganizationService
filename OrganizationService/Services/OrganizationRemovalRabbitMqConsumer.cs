using System.Text;
using Newtonsoft.Json.Linq;
using OrganizationService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrganizationService.Services;

public class OrganizationRemovalRabbitMqConsumer: BackgroundService
    {
        private string _queueName;
        private readonly string _routingKey = "KK.EVENT.ADMIN.organizations.SUCCESS.ORGANIZATION.DELETE"; // Replace with your routing key if needed

        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly OrganizationRemovalRabbitMqSender _sender;
        private readonly ILogPublisher _logPublisher;

        public OrganizationRemovalRabbitMqConsumer(IServiceProvider serviceProvider, IConfiguration configuration, OrganizationRemovalRabbitMqSender sender, ILogPublisher logPublisher)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _sender = sender;
            _logPublisher = logPublisher;
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
            _queueName = _channel.QueueDeclare("keycloak-organization-removal", durable:true, exclusive:false, autoDelete: false, arguments: null);

            // Bind to the topic exchange
            _channel.QueueBind(queue: _queueName,
                               exchange: "amq.topic",
                               routingKey: _routingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
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
            catch (Exception e)
            {
                _logPublisher.SendMessage(new LogMessage
                {
                    ServiceName = "OrganizationService",
                    LogLevel = "Error",
                    Message = $"Failed to receive message. Error: {e.Message}",
                    Timestamp = DateTime.Now,
                });
                Console.WriteLine(e);
                throw;
            }
        }

        private Task HandleMessageAsync(string message)
        {
            // Add your message processing logic here
            Console.WriteLine($"Received message: {message}");
            using (var scope = _serviceProvider.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
                if (JObject.Parse(message).ContainsKey("resourcePath"))
                {
                    var resourcePath = (string)JObject.Parse(message)["resourcePath"]!;
                    var organizationId = new Guid(resourcePath.Replace("organizations/", string.Empty));
                    var schemaName = organizationService.DeleteOrganization(organizationId);
                
                    _sender.SendMessage("organization.delete", schemaName);
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