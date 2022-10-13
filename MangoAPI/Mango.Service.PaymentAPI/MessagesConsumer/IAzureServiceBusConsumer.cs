namespace Mango.Service.PaymentAPI.MessagesConsumer
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}