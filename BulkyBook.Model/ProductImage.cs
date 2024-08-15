namespace BulkyBook.Model
{
    public class ProductImage : BaseModel
    {
        public string ImageUrl { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
