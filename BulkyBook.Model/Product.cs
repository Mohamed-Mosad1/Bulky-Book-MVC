using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string ISBN { get; set; } = null!;
        public string Author { get; set; } = null!;
        [DisplayName("List Price")]
        [Range(1, 10000)]
        public decimal ListPrice { get; set; }
        [DisplayName("Price for 1-50")]
        [Range(1, 10000)]
        public decimal Price { get; set; }
        [DisplayName("Price for 50-100")]
        [Range(1, 10000)]
        public decimal Price50 { get; set; }
        [DisplayName("Price for 100+")]
        [Range(1, 10000)]
        public decimal Price100 { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();


    }
}
