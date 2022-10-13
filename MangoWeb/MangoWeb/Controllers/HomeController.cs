using MangoWeb.Models;
using MangoWeb.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MangoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IProductService productService,
            ICartService cartService,
            ILogger<HomeController> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> productDtos = new List<ProductDto>();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response == null || !response.IsSuccess) return View(productDtos);

            productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            return View(productDtos);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto model = new ProductDto();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");
            if (response == null || !response.IsSuccess) return View(model);

            model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(model);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value
                }

            };

            CartDetailsDto cartDetailsDto = new CartDetailsDto
            {
                ProductId = productDto.ProductId,
                Count = productDto.Count
            };

            var response = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, "");
            if (response == null || !response.IsSuccess) return View();

            cartDetailsDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            List<CartDetailsDto> cartDetailsList = new List<CartDetailsDto>();
            cartDetailsList.Add(cartDetailsDto);
            cartDto.CartDetails = cartDetailsList;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResponse = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);
            if (addToCartResponse == null || !addToCartResponse.IsSuccess) return View(productDto);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
