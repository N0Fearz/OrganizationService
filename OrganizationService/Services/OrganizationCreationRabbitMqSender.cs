using System.Text;
using RabbitMQ.Client;

namespace OrganizationService.Services;

public class OrganizationCreationRabbitMqSender
{
    private IModel _channel;
    private readonly IConfiguration _configuration;

    public OrganizationCreationRabbitMqSender(IConfiguration configuration)
    {
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
        var _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "organization-create-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
    }
    
    public void SendMessage(string routingKey, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "amq.topic", // The topic exchange
            routingKey: routingKey, // Routing key to target specific queues
            basicProperties: null, // Message properties (can add headers, etc.)
            body: body);

        Console.WriteLine($"Message sent to {routingKey}: {message}");
    }
}