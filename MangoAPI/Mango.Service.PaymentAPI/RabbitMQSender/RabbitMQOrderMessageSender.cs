using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Service.PaymentAPI.RabbitMQSender
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private readonly string _paymentExchange;

        private IConnection _connection;

        private const string DIRECT_EXCHANGE = "DirectPayment_Exchange";
        private const string EMAIL_QUEUE = "PaymentEmailUpdateQueueName";
        private const string ORDER_QUEUE = "PaymentOrderUpdateQueueName";
        public RabbitMQPaymentMessageSender(IConfiguration configuration)
        {
            _hostname = configuration.GetValue<string>("RabbitMQ:Hostname");
            _password = configuration.GetValue<string>("RabbitMQ:Password");
            _username = configuration.GetValue<string>("RabbitMQ:Username");
            _paymentExchange = configuration.GetValue<string>("RabbitMQ:PaymentUpdateExchange");
        }

        public void SendMessage(BaseMessage message)
        {
            if (!ConnectionExists()) return;

            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(_paymentExchange, ExchangeType.Fanout, false);

            //Direct
            //channel.ExchangeDeclare(DIRECT_EXCHANGE, ExchangeType.Direct, false);
            //channel.QueueDeclare(ORDER_QUEUE, false, false, false, null);
            //channel.QueueDeclare(EMAIL_QUEUE, false, false, false, null);

            //channel.QueueBind(EMAIL_QUEUE, DIRECT_EXCHANGE, "PaymentEmail");
            //channel.QueueBind(ORDER_QUEUE, DIRECT_EXCHANGE, "PaymentOrder");

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(_paymentExchange, routingKey: "", null, body);

            //Direct
            //channel.BasicPublish(DIRECT_EXCHANGE, routingKey: "PaymentEmail", null, body);
            //channel.BasicPublish(DIRECT_EXCHANGE, routingKey: "PaymentOrder", null, body);
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
