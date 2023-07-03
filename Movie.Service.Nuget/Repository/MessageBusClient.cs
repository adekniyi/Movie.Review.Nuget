using System;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Movie.Service.Nuget.Interface;
using RabbitMQ.Client;
using Movie.Service.Nuget.Extension;
#if NET5_0
    using Newtonsoft.Json;
#endif

namespace Movie.Service.Nuget.Repository
{
	public class MessageBusClient : IMessageBusClient
    {
        private  IConnection _connection;
        private IModel _channel;

        public IConfiguration _config { get; set; }

        public string _routingKey;
        public string _exchange;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;

        }

        public void Initialize(string exchange)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _config["RabbiMQ:Host"], Port = int.Parse(_config["RabbiMQ:Port"]) };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);

                _exchange = exchange;

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("Listenting on message bus: queue name should follow");
            }
            catch (Exception ex)
            {

            }
        }

        public void Publish(dynamic model, string routingKey)
        {
#if NET5_0
            var message = JsonConvert.SerializeObject(model);
#elif NET7_0
            var message = JsonSerializer.Serialize(model);
#endif
            if (_connection.IsOpen)
            {
                Console.WriteLine("Message bus is connected");
                SendMessage(message, routingKey);
            }
            else
            {
                Console.WriteLine("Message bus closed");
            }
        }

        public void Publish(Func<Task> model, string routingKey)
        {
            //var message = JsonSerializer.Serialize(model);

            if (_connection.IsOpen)
            {
                Console.WriteLine("Message bus is connected");
                var body = CustomLinq.SerializeFunction(model);

                _channel.BasicPublish(exchange: _exchange, routingKey: routingKey, basicProperties: null, body: body);
                Console.WriteLine($"Message sent {model}");
            }
            else
            {
                Console.WriteLine("Message bus closed");
            }
        }


        private void SendMessage(string message, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: _exchange, routingKey: routingKey, basicProperties: null, body: body);
            Console.WriteLine($"Message sent {message}");

        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Shut down");
        }
    }
}

