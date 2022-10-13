using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Service.ShoppingCartAPI.Dtos
{
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public virtual CartHeaderDto CartHeader { get; set; }
        public virtual ProductDto Product { get; set; }
    }
}
