using MangoWeb.Models;
using MangoWeb.Services.IServices;

namespace MangoWeb.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _httpClient;

        public ProductService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.POST,
                Data = productDto,
                Url = Settings.ProductAPIBase + $"/api/products",
                AccessToken = token
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.DELETE,
                Data = id,
                Url = Settings.ProductAPIBase + $"/api/products/RemoveCart",
                AccessToken = token
            });
        }

        public async Task<T> GetAllProductsAsync<T>(string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.GET,
                Url = Settings.ProductAPIBase + $"/api/products",
                AccessToken = token
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.GET,
                Url = Settings.ProductAPIBase + $"/api/products/{id}",
                AccessToken = token
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = APIType.PUT,
                Data = productDto,
                Url = Settings.ProductAPIBase + $"/api/products",
                AccessToken = token
            });
        }
    }
}
