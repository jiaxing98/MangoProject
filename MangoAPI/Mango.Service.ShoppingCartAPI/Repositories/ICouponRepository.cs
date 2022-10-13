using Mango.Service.ShoppingCartAPI.Dtos;

namespace Mango.Service.ShoppingCartAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName);
    }
}
