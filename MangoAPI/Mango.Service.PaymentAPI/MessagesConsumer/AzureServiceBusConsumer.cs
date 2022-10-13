using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.Service.PaymentAPI.Messages;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.PaymentAPI.MessagesConsumer
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnectionString;
        private readonly string _orderPaymentProcessTopic;
        private readonly string _orderUpdatePaymentTopic;
        private readonly string _orderPaymentSubscription;

        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;
        private ServiceBusProcessor _orderPaymentProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IProcessPayment processPayment, IMessageBus messageBus)
        {
            _configuration = configuration;
            _processPayment = processPayment;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBus:ConnectionString");
            _orderPaymentProcessTopic = _configuration.GetValue<string>("ServiceBus:OrderPaymentProcessTopic");
            _orderUpdatePaymentTopic = _configuration.GetValue<string>("ServiceBus:OrderUpdatePaymentTopic");
            _orderPaymentSubscription = _configuration.GetValue<string>("ServiceBus:OrderPaymentSubscription");

            _messageBus = messageBus;
            var client = new ServiceBusClient(_serviceBusConnectionString);
            _orderPaymentProcessor = client.CreateProcessor(_orderPaymentProcessTopic, _orderPaymentSubscription);
        }

        public async Task Start()
        {
            _orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
            _orderPaymentProcessor.ProcessErrorAsync += OnErrorReceived;

            await _orderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _orderPaymentProcessor.StopProcessingAsync();
            await _orderPaymentProcessor.DisposeAsync();
        }

        private Task OnErrorReceived(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new UpdatePaymentResultMessage()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email =paymentRequestMessage.Email
            };

            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
