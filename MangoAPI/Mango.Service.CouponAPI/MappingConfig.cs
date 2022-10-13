using AutoMapper;
using Mango.Service.CouponAPI.Dtos;
using Mango.Service.CouponAPI.Models;

namespace Mango.Service.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Coupon, CouponDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
