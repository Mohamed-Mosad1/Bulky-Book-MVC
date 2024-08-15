// Ignore Spelling: app

using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.DAL.Specifications.OrderSpecs
{
    public class OrdersWithOrderItemsSpec : BaseSpecifications<Order>
    {
        public OrdersWithOrderItemsSpec(string userOrOrderId, bool includeUser = false, bool includeOrderItems = false)
            : base(o => string.IsNullOrEmpty(userOrOrderId) || o.AppUserId == userOrOrderId || o.Id == userOrOrderId)
        {
            ApplyIncludes(includeUser, includeOrderItems);
            AddOrderByDesc(o => o.OrderDate);
        }

        private void ApplyIncludes(bool includeUser, bool includeOrderItems)
        {
            if (includeUser)
                AddInclude(x => x.AppUser);

            if (includeOrderItems)
                AddInclude(x => x.OrderItems);

            if (includeUser && includeOrderItems)
            {
                AddInclude(x => x.AppUser);
                AddInclude(x => x.OrderItems);
            }
        }
    }
}
