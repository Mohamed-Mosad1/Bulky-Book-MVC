using BulkyBook.Model.OrdersAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
