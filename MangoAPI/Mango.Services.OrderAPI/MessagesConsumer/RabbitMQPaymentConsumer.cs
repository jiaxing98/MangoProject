
using Mango.Service.OrderAPI.Messages;
using Mango.Service.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Service.OrderAPI.MessagesConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly string _paymentExchange;
        private readonly OrderRepository _orderRepository;

        private IConnection _connection;
        private IModel _channel;
        private string _queueName = "";

        private const string DIRECT_EXCHANGE = "DirectPayment_Exchange";
        private const string ORDER_QUEUE = "PaymentOrderUpdateQueueName";

        public RabbitMQPaymentConsumer(IConfiguration configuration, OrderRepository orderRepository)
        {
            _paymentExchange = configuration.GetValue<string>("RabbitMQ:PaymentUpdateExchange");
            _orderRepository = orderRepository;

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("RabbitMQ:Hostname"),
                UserName = configuration.GetValue<string>("RabbitMQ:Username"),
                Password = configuration.GetValue<string>("RabbitMQ:Password")
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //Fanout
            _channel.ExchangeDeclare(_paymentExchange, ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, _paymentExchange, "");

            //Direct
            //_channel.ExchangeDeclare(_paymentExchange, ExchangeType.Direct);
            //_channel.QueueDeclare(ORDER_QUEUE, false, false, false, null);
            //_channel.QueueBind(ORDER_QUEUE, DIRECT_EXCHANGE, "PaymentOrder");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, eventArgs) =>
            {
                var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            //Fanout
            _channel.BasicConsume(_queueName, false, consumer);

            //Direct
            //_channel.BasicConsume(ORDER_QUEUE, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {

            try
            {
                await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
