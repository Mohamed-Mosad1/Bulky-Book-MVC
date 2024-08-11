using System.Runtime.Serialization;

namespace BulkyBook.Model.OrdersAggregate
{
    public enum PaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Approved")]
        Approved,

        [EnumMember(Value = "ApprovedForDelayedPayment")]
        ApprovedForDelayedPayment,

        [EnumMember(Value = "Refunded")]
        Refunded,

        [EnumMember(Value = "Rejected")]
        Rejected,

        [EnumMember(Value = "Cancelled")]
        Cancelled
    }
}
