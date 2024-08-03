namespace BulkyBook.Model.OrdersAggregate
{
    public class OrderItem : BaseModel
    {
        private OrderItem() { }

        public OrderItem(Product productOrdered, decimal price, int quantity)
        {
            ProductOrdered = productOrdered;
            Price = price;
            Quantity = quantity;
        }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public Product ProductOrdered { get; set; } = null!;
    }
}
