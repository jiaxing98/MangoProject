using Mango.Service.ShoppingCartAPI.Dtos;
using Newtonsoft.Json;

namespace Mango.Service.ShoppingCartAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponDto> GetCoupon(string couponName)
        {
            var response = await _client.GetAsync($"/api/coupon/{couponName}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (!responseObj.IsSuccess) return new CouponDto();

            return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseObj.Result));
        }
    }
}
