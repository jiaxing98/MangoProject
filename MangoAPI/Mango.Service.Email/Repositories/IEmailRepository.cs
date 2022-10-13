using Mango.Service.Email.Messages;

namespace Mango.Service.Email.Repositories
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
