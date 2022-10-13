using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Service.OrderAPI.Messages;
using Mango.Service.OrderAPI.Models;
using Mango.Service.OrderAPI.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.OrderAPI.MessagesConsumer
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnectionString;
        private readonly string _checkoutMessageTopic;
        private readonly string _checkoutSubscription;
        private readonly string _orderPaymentProcessTopic;
        private readonly string _orderUpdatePaymentTopic;

        private readonly IMessageBus _messageBus;
        private readonly OrderRepository _orderRepository;
        private ServiceBusProcessor _checkoutProcessor;
        private ServiceBusProcessor _orderUpdatePaymentProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, OrderRepository orderRepository)
        {
            _configuration = configuration;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBus:ConnectionString");
            _checkoutMessageTopic = _configuration.GetValue<string>("ServiceBus:CheckoutMessageTopic");
            _checkoutSubscription = _configuration.GetValue<string>("ServiceBus:CheckoutSubscription");
            _orderPaymentProcessTopic = _configuration.GetValue<string>("ServiceBus:OrderPaymentProcessTopic");
            _orderUpdatePaymentTopic = _configuration.GetValue<string>("ServiceBus:OrderUpdatePaymentTopic");

            _messageBus = messageBus;
            _orderRepository = orderRepository;

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _checkoutProcessor = client.CreateProcessor(_checkoutMessageTopic, _checkoutSubscription);
            _orderUpdatePaymentProcessor = client.CreateProcessor(_orderUpdatePaymentTopic, _checkoutSubscription);
        }

        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            _checkoutProcessor.ProcessErrorAsync += OnErrorReceived;
            await _checkoutProcessor.StartProcessingAsync();

            _orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderUpdatePaymentReceived;
            _orderUpdatePaymentProcessor.ProcessErrorAsync += OnErrorReceived;
            await _orderUpdatePaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();

            await _orderUpdatePaymentProcessor.StopProcessingAsync();
            await _orderUpdatePaymentProcessor.DisposeAsync();
        }

        private Task OnErrorReceived(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
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
                await _messageBus.PublishMessage(paymentRequestMessage, _orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnOrderUpdatePaymentReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
