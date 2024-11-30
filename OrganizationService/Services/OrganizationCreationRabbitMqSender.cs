using System.Text;
using RabbitMQ.Client;

namespace OrganizationService.Services;

public class OrganizationCreationRabbitMqSender
{
    private IConnection _connection;
    private IModel _channel;
    private readonly IConfiguration _configuration;
    private IServiceProvider _serviceProvider;

    public OrganizationCreationRabbitMqSender(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
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

        _channel.ExchangeDeclare(
            exchange: "amq.topic", 
            type: ExchangeType.Topic, 
            durable: true);
    }
    
    public void SendMessage(string routingKey, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "amq.topic", // The topic exchange
            routingKey: "create.organization", // Routing key to target specific queues
            basicProperties: null, // Message properties (can add headers, etc.)
            body: body);

        Console.WriteLine($"Message sent to {routingKey}: {message}");
    }
}