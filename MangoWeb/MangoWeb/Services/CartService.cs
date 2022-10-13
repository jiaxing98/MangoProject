using MangoWeb.Models;
using MangoWeb.Services.IServices;

namespace MangoWeb.Services
{
    public class CartService : BaseService, ICartService
    {
        private readonly IHttpClientFactory _httpClient;

        public CartService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.POST,
                Data = cartDto,
                Url = Settings.CartAPIBase + $"/api/cart/AddCart",
                AccessToken = token
            });
        }

        public async Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.POST,
                Data = cartDto,
                Url = Settings.CartAPIBase + $"/api/cart/ApplyCoupon",
                AccessToken = token
            });
        }

        public async Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.POST,
                Data = cartHeaderDto,
                Url = Settings.CartAPIBase + $"/api/cart/Checkout",
                AccessToken = token
            });
        }

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.GET,
                Url = Settings.CartAPIBase + $"/api/cart/GetCart/{userId}",
                AccessToken = token
            });
        }

        public async Task<T> RemoveCouponAsync<T>(string userId, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.POST,
                Data = userId,
                Url = Settings.CartAPIBase + $"/api/cart/RemoveCoupon",
                AccessToken = token
            });
        }

        public async Task<T> RemoveFromCartAsync<T>(int cartId, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.DELETE,
                Data = cartId,
                Url = Settings.CartAPIBase + $"/api/cart/RemoveCart",
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.PUT,
                Data = cartDto,
                Url = Settings.CartAPIBase + $"/api/cart/UpdateCart",
                AccessToken = token
            });
        }
    }
}
