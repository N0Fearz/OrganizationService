﻿using System.Text;
using System.Text.Json;
using OrganizationService.Models;
using RabbitMQ.Client;

namespace OrganizationService.Services;

public class LogPublisher: ILogPublisher
{
    private readonly IConfiguration _configuration;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    
    public LogPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };

        // Establish connection and create a channel
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }
    
    public void SendMessage(LogMessage logMessage)
    {
        var message = JsonSerializer.Serialize(logMessage);

        var body = Encoding.UTF8.GetBytes(message); 
        
        var routingKey = $"logs.{logMessage.LogLevel.ToLower()}";
        _channel.BasicPublish(
            exchange: "amq.topic", // The topic exchange
            routingKey: routingKey, // Routing key to target specific queues
            basicProperties: null, // Message properties (can add headers, etc.)
            body: body);

        Console.WriteLine($"Message sent to {routingKey}: {message}");
    }
    
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}