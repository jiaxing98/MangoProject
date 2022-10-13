using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Service.OrderAPI.RabbitMQSender
{
    public class RabbitMQOrderMessageSender : IRabbitMQOrderMessageSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;

        private IConnection _connection;

        public RabbitMQOrderMessageSender(IConfiguration configuration)
        {
            _hostname = configuration.GetValue<string>("RabbitMQ:Hostname");
            _password = configuration.GetValue<string>("RabbitMQ:Password");
            _username = configuration.GetValue<string>("RabbitMQ:Username");
        }

        public void SendMessage(BaseMessage message, string queueName)
        {
            if (!ConnectionExists()) return;

            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish("", queueName, null, body);
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null) return true;

            CreateConnection();
            return _connection != null;
        }
    }
}
