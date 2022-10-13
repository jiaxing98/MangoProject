using Azure.Messaging.ServiceBus;
using Mango.Service.Email.Messages;
using Mango.Service.Email.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.Email.MessagesConsumer
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnectionString;
        private readonly string _orderUpdatePaymentTopic;
        private readonly string _emailSubscription;

        private readonly EmailRepository _emailRepository;
        private ServiceBusProcessor _orderUpdatePaymentProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailRepository emailRepository)
        {
            _configuration = configuration;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBus:ConnectionString");
            _orderUpdatePaymentTopic = _configuration.GetValue<string>("ServiceBus:OrderUpdatePaymentTopic");
            _emailSubscription = _configuration.GetValue<string>("ServiceBus:EmailSubscriptions");

            _emailRepository = emailRepository;

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _orderUpdatePaymentProcessor = client.CreateProcessor(_orderUpdatePaymentTopic, _emailSubscription);
        }

        public async Task Start()
        {
            _orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderUpdatePaymentReceived;
            _orderUpdatePaymentProcessor.ProcessErrorAsync += OnErrorReceived;
            await _orderUpdatePaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            _orderUpdatePaymentProcessor.ProcessMessageAsync -= OnOrderUpdatePaymentReceived;
            _orderUpdatePaymentProcessor.ProcessErrorAsync -= OnErrorReceived;
            await _orderUpdatePaymentProcessor.StopProcessingAsync();
            await _orderUpdatePaymentProcessor.DisposeAsync();
        }

        private Task OnErrorReceived(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderUpdatePaymentReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage updatedMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            try
            {
                await _emailRepository.SendAndLogEmail(updatedMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
