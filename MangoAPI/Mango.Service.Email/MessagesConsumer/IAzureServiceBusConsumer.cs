namespace Mango.Service.Email.MessagesConsumer
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}