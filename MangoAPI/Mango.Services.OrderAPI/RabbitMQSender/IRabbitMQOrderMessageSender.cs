using Mango.MessageBus;

namespace Mango.Service.OrderAPI.RabbitMQSender
{
    public interface IRabbitMQOrderMessageSender
    {
        void SendMessage(BaseMessage message, String queueName);
    }
}
