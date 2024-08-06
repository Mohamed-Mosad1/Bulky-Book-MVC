// Ignore Spelling: app

using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.DAL.Specifications.OrderSpecs
{
    public class OrdersWithOrderItemsSpec : BaseSpecifications<Order>
    {
        public OrdersWithOrderItemsSpec(string? userOrOrderId)
            : base(o => o.AppUserId == userOrOrderId || o.Id == userOrOrderId)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.AppUser);
            //AddOrderByDesc(o => o.OrderDate);

        }
    }
}
