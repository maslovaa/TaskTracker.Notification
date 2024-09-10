using Services.DTO;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace Services.Consumers
{
    public class MessageConsumer : IMessageConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageConsumer(IConfiguration configuration)
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMqConfig["HostName"],
                Port = int.Parse(rabbitMqConfig["Port"]),
                UserName = rabbitMqConfig["UserName"],
                Password = rabbitMqConfig["Password"],
                VirtualHost = rabbitMqConfig["VirtualHost"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "MessageQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Получено сообщение: {message}");
            };

            _channel.BasicConsume(queue: "MessageQueue", autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
