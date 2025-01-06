using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrganizationService.Services;

public class OrganizationCheckRabbitMqconsumer : BackgroundService
{
    // Replace with your routing key if needed

    private IConnection _connection;
    private IModel _channel;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public OrganizationCheckRabbitMqconsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
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
        
        _channel.QueueDeclare("request-queue", exclusive: false);

    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => StopRabbitMQ());

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {ea.BasicProperties.CorrelationId}");

            // Process the message
            using (var scope = _serviceProvider.CreateScope())
            {
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                var organizationService = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
                var response = organizationService.CheckOrganization(new Guid(message));
                var newBody = Encoding.UTF8.GetBytes(response);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body:newBody);
                Console.WriteLine($"Sent message: {props.ReplyTo}");
            }
        };

        _channel.BasicConsume(queue: "request-queue",
                              autoAck: true,
                              consumer: consumer);

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