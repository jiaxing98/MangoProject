using Mango.MessageBus;

namespace Mango.Service.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQCartMessageSender
    {
        void SendMessage(BaseMessage message, String queueName);
    }
}
