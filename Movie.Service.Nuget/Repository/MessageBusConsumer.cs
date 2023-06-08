using System;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using RabbitMQ.Client;
using System.Text;
using Movie.Service.Nuget.Interface;
using Microsoft.Extensions.Configuration;

namespace Movie.Service.Nuget.Repository
{
	public class MessageBusConsumer<T> where T : IEventProcessor
    {
        private readonly IConfiguration _config;

        public IConnection _connection;
        public IModel _channel;
        private readonly T _eventProcessor;

        private string _queue;
        public MessageBusConsumer(T eventProcessor, IConfiguration config)
        {
            // _bus = bus;
            _eventProcessor = eventProcessor;
            _config = config;
        }


        public void InitializeRMQ(string exchange, string queue, string routingKey)
        {
            var factory = new ConnectionFactory() { HostName = _config["RabbiMQ:Host"], Port = int.Parse(_config["RabbiMQ:Port"]) };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout); //"trigger_movie"

                _channel.QueueDeclare(queue: queue, //"trigger_review_queue"
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                _queue = queue;

                _channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);//"trigger_review_queue", "trigger_movie", "trigger_movie_create"

                Console.WriteLine("Listenting on message bus: queue name should follow");
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("Connected to massage bus");
            }
            catch (Exception ex)
            {

            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Shut down");
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (moduleHandle, e) =>
            {
                var body = e.Body;

                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                Console.WriteLine($"Message receieved {notificationMessage}");

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queue, autoAck: true, consumer: consumer);
        }


        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}

