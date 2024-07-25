using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Model.ViewModels
{
    public class ProductVM
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

        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [DisplayName("Product Image")]
        public List<ProductImage>? ProductImages { get; set; }
    }
}
