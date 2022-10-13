using MangoWeb.Models;
using MangoWeb.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MangoWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new List<ProductDto>();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);
            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response!.Result!)!);
            
            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.CreateProductAsync<ResponseDto>(model, accessToken);
            if (response == null || !response.IsSuccess) return View(model);
            else return RedirectToAction(nameof(ProductIndex));
        }

        public async Task<IActionResult> ProductUpdate(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (response == null || !response.IsSuccess) return NotFound();

            ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response!.Result!)!)!;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductUpdate(ProductDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.UpdateProductAsync<ResponseDto>(model, accessToken);
            if (response == null || !response.IsSuccess) return View(model);
            else return RedirectToAction(nameof(ProductIndex));
        }

        [Authorize(Roles = Settings.ADMIN)]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (response == null || !response.IsSuccess) return NotFound();

            ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response!.Result!)!)!;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Settings.ADMIN)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId, accessToken);
            if (response == null || !response.IsSuccess) return View(model);
            else return RedirectToAction(nameof(ProductIndex));
        }
    }
}
