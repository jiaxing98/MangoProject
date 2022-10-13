using Mango.Service.CouponAPI.Dtos;

namespace Mango.Service.CouponAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
