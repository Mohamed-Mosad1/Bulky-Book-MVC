// Ignore Spelling: App

using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Model.ViewModels
{
    public class ShoppingCartItemVM
    {
        public string Id { get; set; } = null!;

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Quantity { get; set; }

        public string AppUserId { get; set; } = null!;

        public int ProductId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductImageVM>? ProductImages { get; set; } = new List<ProductImageVM>();
    }
}
