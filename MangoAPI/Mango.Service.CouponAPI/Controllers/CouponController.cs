using Mango.Service.CouponAPI.Dtos;
using Mango.Service.CouponAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository _couponRepository;
        protected ResponseDto _response;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            this._response = new ResponseDto();
        }

        [HttpGet("{couponCode}")]
        public async Task<object> GetDiscountForCode(string couponCode)
        {
            try
            {
                CouponDto couponDto = await _couponRepository.GetCouponByCode(couponCode);
                _response.Result = couponDto;
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
