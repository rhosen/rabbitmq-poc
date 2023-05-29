using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RabbitService
{
    public partial class Service1 : ServiceBase
    {
        private IConnection _connection;
        private IModel _channel;
        private StreamWriter _logWriter;
        private string _logFilePath;
        private string _serviceToApplicationQueue = "ServiceToApplicationQueue";
        private string _applicationToServiceQueue = "ApplicationToServiceQueue";

        #region Initialization

        public Service1()
        {
            InitializeComponent();
        }

        private void InitializeConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override async void OnStart(string[] args)
        {
            InitializeLogWriter();
            InitializeConnection();
            StartMessageConsumption();
            await SendMessageToClientAfterDelay("This is a scheduled message from server");
        }

        protected override void OnStop()
        {
            _channel.Close();
            _connection.Close();
            CloseLogWriter();
        }

        #endregion Initialization

        #region Consumption

        private void StartMessageConsumption()
        {
            _channel.QueueDeclare(queue: _applicationToServiceQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ProcessReceivedMessage;
            _channel.BasicConsume(queue: _applicationToServiceQueue, noAck: false, consumer: consumer);
        }

        private void ProcessReceivedMessage(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            LogMessage($"Received message from application: {message}");
            _channel.BasicAck(e.DeliveryTag, multiple: false);
        }

        #endregion

        #region Production

        private void SendMessageToApplication(string message = "Hello, application!")
        {
            _channel.QueueDeclare(queue: _serviceToApplicationQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: _serviceToApplicationQueue, basicProperties: null, body: body);
            LogMessage($"Sent message to application: {message}");
        }

        private async Task SendMessageToClientAfterDelay(string message, int delaySeconds = 30)
        {
            LogMessage($"This method will keep sending message to the client after a time interval of: {delaySeconds} seconds");
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                SendMessageToApplication(message);
            }
        }

        #endregion

        #region Logging

        private void InitializeLogWriter()
        {
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            _logWriter = new StreamWriter(_logFilePath, true);
        }

        private void CloseLogWriter()
        {
            _logWriter.Close();
            _logWriter.Dispose();
        }

        private void LogMessage(string message)
        {
            string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            _logWriter.WriteLine(formattedMessage);
            _logWriter.Flush();
        }

        #endregion

    }

}
