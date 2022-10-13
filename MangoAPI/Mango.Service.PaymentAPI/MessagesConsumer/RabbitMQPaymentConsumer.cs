using Mango.PaymentProcessor;
using Mango.Service.PaymentAPI.Messages;
using Mango.Service.PaymentAPI.RabbitMQSender;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Service.PaymentAPI.MessagesConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly string _orderQueue;
        private readonly IRabbitMQPaymentMessageSender _rabbitMQPaymentMessageSender;
        private readonly IProcessPayment _processPayment;

        private IConnection _connection;
        private IModel _channel;

        public RabbitMQPaymentConsumer(IConfiguration configuration, IRabbitMQPaymentMessageSender rabbitMQPaymentMessageSender, IProcessPayment processPayment)
        {
            _orderQueue = configuration.GetValue<string>("RabbitMQ:OrderPaymentQueue");
            _rabbitMQPaymentMessageSender = rabbitMQPaymentMessageSender;
            _processPayment = processPayment;

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("RabbitMQ:Hostname"),
                UserName = configuration.GetValue<string>("RabbitMQ:Username"),
                Password = configuration.GetValue<string>("RabbitMQ:Password")
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_orderQueue, false, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, eventArgs) =>
            {
                var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);
                HandleMessage(paymentRequestMessage).GetAwaiter().GetResult();
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            _channel.BasicConsume(_orderQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new UpdatePaymentResultMessage()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email
            };

            try
            {
                _rabbitMQPaymentMessageSender.SendMessage(updatePaymentResultMessage);
                //await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentTopic);
                //await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
