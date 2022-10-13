using Mango.MessageBus;
using Mango.Service.ShoppingCartAPI.Dtos;
using Mango.Service.ShoppingCartAPI.Messages;
using Mango.Service.ShoppingCartAPI.RabbitMQSender;
using Mango.Service.ShoppingCartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        
        private readonly IMessageBus _messageBus;
        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;

        private readonly string _checkoutQueue;

        protected ResponseDto _response;

        public CartController(IConfiguration configuration, ICartRepository cartRepository, ICouponRepository couponRepository, IMessageBus messageBus, IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _checkoutQueue = configuration.GetValue<string>("RabbitMQ:CheckoutMessageQueue");

            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _messageBus = messageBus;
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
            _response = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto createdCartDto = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = createdCartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto updatedCartDto = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = updatedCartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }

        [HttpDelete("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeaderDto.UserId);
                if (cartDto == null) return BadRequest();

                if (!string.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
                {
                    CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeaderDto.CouponCode);
                    if(checkoutHeaderDto.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>()
                        {
                            "Coupon Price has changed, please confirm!"
                        };
                        _response.DisplayMessage = "Coupon Price has changed, please confirm!";
                        return _response;
                    }
                }

                checkoutHeaderDto.CartDetails = cartDto.CartDetails;
                //Azure
                //await _messageBus.PublishMessage(checkoutHeaderDto, TOPIC_NAME);

                //RabbitMQ
                _rabbitMQCartMessageSender.SendMessage(checkoutHeaderDto, _checkoutQueue);
                await _cartRepository.ClearCart(checkoutHeaderDto.UserId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }
    }
}
