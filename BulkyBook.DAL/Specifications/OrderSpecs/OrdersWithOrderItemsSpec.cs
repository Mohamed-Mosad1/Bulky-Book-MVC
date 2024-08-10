// Ignore Spelling: app

using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.DAL.Specifications.OrderSpecs
{
    public class OrdersWithOrderItemsSpec : BaseSpecifications<Order>
    {
        public OrdersWithOrderItemsSpec(string? userOrOrderId, bool includeUser = false, bool includeOrderItems = false)
            : base(o => o.AppUserId == userOrOrderId || o.Id == userOrOrderId)
        {
            if (includeUser)
                AddInclude(x => x.AppUser);
            else if (includeOrderItems)
                AddInclude(x => x.OrderItems);

            if (includeUser && includeOrderItems)
            {
                AddInclude(x => x.AppUser);
                AddInclude(x => x.OrderItems);
            }
            //AddOrderByDesc(o => o.OrderDate);

        }
    }
}
