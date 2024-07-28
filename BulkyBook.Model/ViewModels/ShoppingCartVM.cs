using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Model.ViewModels
{
    public class ShoppingCartVM
    {
        public int Id { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Quantity { get; set; }

        public string AppUserId { get; set; } = null!;

        public int ProductId { get; set; }
    }
}
