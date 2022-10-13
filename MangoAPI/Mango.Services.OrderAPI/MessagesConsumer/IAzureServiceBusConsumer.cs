namespace Mango.Service.OrderAPI.MessagesConsumer
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}