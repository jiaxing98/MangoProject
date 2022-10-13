using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Service.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        [ForeignKey("CartHeaderId")] 
        public virtual CartHeader CartHeader { get; set; }
        [ForeignKey("ProductId")] 
        public virtual Product Product { get; set; }
    }
}
