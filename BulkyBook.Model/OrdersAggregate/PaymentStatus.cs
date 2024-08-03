using System.Runtime.Serialization;

namespace BulkyBook.Model.OrdersAggregate
{
    public enum PaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Approved")]
        Approved,

        [EnumMember(Value = "Approved For Delayed Payment")]
        ApprovedForDelayedPayment,

        [EnumMember(Value = "Rejected")]
        Rejected
    }
}
