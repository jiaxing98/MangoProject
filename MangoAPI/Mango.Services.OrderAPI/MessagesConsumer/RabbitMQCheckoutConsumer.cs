using Mango.Service.OrderAPI.Messages;
using Mango.Service.OrderAPI.Models;
using Mango.Service.OrderAPI.RabbitMQSender;
using Mango.Service.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Service.OrderAPI.MessagesConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _checkoutQueue;
        private readonly string _orderQueue;

        private readonly OrderRepository _orderRepository;
        private readonly IRabbitMQOrderMessageSender _rabbitMQOrderMessageSender;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMQCheckoutConsumer(IConfiguration configuration, OrderRepository orderRepository, IRabbitMQOrderMessageSender rabbitMQOrderMessageSender)
        {
            _hostname = configuration.GetValue<string>("RabbitMQ:Hostname");
            _username = configuration.GetValue<string>("RabbitMQ:Username");
            _password = configuration.GetValue<string>("RabbitMQ:Password");
            _checkoutQueue = configuration.GetValue<string>("RabbitMQ:CheckoutMessageQueue");
            _orderQueue = configuration.GetValue<string>("RabbitMQ:OrderPaymentQueue");

            _orderRepository = orderRepository;
            _rabbitMQOrderMessageSender = rabbitMQOrderMessageSender;

            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_checkoutQueue, false, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, eventArgs) =>
            {
                var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content);
                HandleMessage(checkoutHeaderDto).GetAwaiter().GetResult();
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            _channel.BasicConsume(_checkoutQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CheckoutHeaderDto checkoutHeaderDto)
        {
            OrderHeader orderHeader = new OrderHeader()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickUpDateTime = checkoutHeaderDto.PickUpDateTime
            };

            foreach (var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = detailList.ProductId,
                    ProductName = detailList.Product.Name,
                    Price = detailList.Product.Price,
                    Count = detailList.Count
                };

                orderHeader.CartTotalItems += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new PaymentRequestMessage()
            {
                Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.Email
            };

            try
            {
                _rabbitMQOrderMessageSender.SendMessage(paymentRequestMessage, _orderQueue);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
