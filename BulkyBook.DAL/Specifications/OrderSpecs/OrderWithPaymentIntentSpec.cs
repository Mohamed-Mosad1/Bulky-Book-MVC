using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.DAL.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpec : BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpec(string paymentIntentId)
            : base(order => order.PaymentInstantId == paymentIntentId)
        {

        }
    }
}
