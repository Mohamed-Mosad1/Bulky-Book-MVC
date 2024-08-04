namespace BulkyBook.Model.OrdersAggregate
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered() { }
        public ProductItemOrdered(int productId, string productTitle, string? imageUrl)
        {
            ProductId = productId;
            ProductTitle = productTitle;
            ImageUrl = imageUrl;
        }

        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
