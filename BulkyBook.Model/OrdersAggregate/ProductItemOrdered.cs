namespace BulkyBook.Model.OrdersAggregate
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered() { }
        public ProductItemOrdered(int productId, string productTitle, decimal productPrice, string? imageUrl)
        {
            ProductId = productId;
            ProductTitle = productTitle;
            ProductPrice = productPrice;
            ImageUrl = imageUrl;
        }

        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
