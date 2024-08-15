using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.DAL.Specifications.OrderSpecs
{
    public class OrderBySessionIdSpec : BaseSpecifications<Order>
    {
        public OrderBySessionIdSpec(string sessionId)
            : base(order => order.SessionId == sessionId)
        {

        }
    }
}
