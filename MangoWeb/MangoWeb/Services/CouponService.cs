using MangoWeb.Models;
using MangoWeb.Services.IServices;

namespace MangoWeb.Services
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpClientFactory _httpClient;

        public CouponService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetCouponAsync<T>(string couponCode, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.GET,
                Url = Settings.CouponAPIBase + $"/api/coupon/{couponCode}",
                AccessToken = token
            });
        }
    }
}
