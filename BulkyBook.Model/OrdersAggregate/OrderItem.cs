namespace BulkyBook.Model.OrdersAggregate
{
    public class OrderItem : BaseModel
    {
        public OrderItem() { }

        public OrderItem(ProductItemOrdered productOrdered, decimal price, int quantity)
        {
            ProductOrdered = productOrdered;
            Price = price;
            Quantity = quantity;
        }
        public ProductItemOrdered ProductOrdered { get; set; } = null!;

        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
