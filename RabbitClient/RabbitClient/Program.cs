using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitClient
{
    internal class Program
    {

        #region Initialization

        private IConnection _connection;
        private IModel _channel;
        static string _serviceToApplicationQueue = "ServiceToApplicationQueue";
        static string _applicationToServiceQueue = "ApplicationToServiceQueue";

        public Program()
        {
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        #endregion

        static async Task Main()
        {
            Program p = new Program();
            await p.RunApplication();
        }
        private async Task RunApplication()
        {
            StartMessageConsumption();
            await SendMessageToClientAfterDelay();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        #region Consumption

        private void StartMessageConsumption()
        {
            _channel.QueueDeclare(queue: _serviceToApplicationQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ProcessReceivedMessage;
            _channel.BasicConsume(queue: _serviceToApplicationQueue, noAck: false, consumer: consumer);
        }

        private void ProcessReceivedMessage(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            LogMessage($"Received message from service: {message}");
            _channel.BasicAck(e.DeliveryTag, multiple: false);
            LogMessage("Acknowledgment sent to the server.");
        }

        #endregion

        #region Production

        private void SendMessage(string message = "Hello, Serivice")
        {
            _channel.QueueDeclare(queue: _applicationToServiceQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: _applicationToServiceQueue, basicProperties: null, body: body);
            LogMessage($"Sent message to service: {message}");
        }

        private async Task SendMessageToClientAfterDelay()
        {
            int delaySeconds = 10;
            int counter = 1;
            LogMessage($"This method will keep sending message to the server after a time interval of: {delaySeconds} seconds");
            while (true)
            {
                var message = $"Hello, Serivice {counter}";
                await Task.Delay(TimeSpan.FromMilliseconds(delaySeconds));
                SendMessage(message);
                counter++;
            }
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.WriteLine(formattedMessage);
        }

        #endregion

    }
}
