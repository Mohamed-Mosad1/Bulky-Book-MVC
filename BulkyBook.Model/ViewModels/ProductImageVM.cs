namespace BulkyBook.Model.ViewModels
{
    public class ProductImageVM
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;

        public int ProductId { get; set; }
    }
}
