using AutoMapper;
using Mango.Service.ProductAPI.Dtos;
using Mango.Service.ProductAPI.Models;

namespace Mango.Service.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
