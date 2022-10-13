using Mango.MessageBus;

namespace Mango.Service.PaymentAPI.RabbitMQSender
{
    public interface IRabbitMQPaymentMessageSender
    {
        void SendMessage(BaseMessage message);
    }
}
