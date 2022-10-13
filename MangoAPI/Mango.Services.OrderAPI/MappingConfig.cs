using AutoMapper;
//using Mango.Service.OrderAPI.Dtos;
using Mango.Service.OrderAPI.Models;

namespace Mango.Service.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<Coupon, CouponDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
