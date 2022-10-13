using MangoWeb.Models;

namespace MangoWeb.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseDto { get; set; }
        Task<T?> SendAsync<T>(APIRequest apiRequest);
    }
}
