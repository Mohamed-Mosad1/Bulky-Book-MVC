using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.DAL.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpec : BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpec(string sessionId)
            : base(order => order.SessionId == sessionId)
        {

        }
    }
}
