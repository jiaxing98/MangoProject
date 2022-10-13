using System.ComponentModel.DataAnnotations;

namespace MangoWeb.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        [Range(1, 100)]
        public int Count { get; set; } = 1;
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
