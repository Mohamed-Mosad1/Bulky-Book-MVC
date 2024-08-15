using System.Runtime.Serialization;

namespace BulkyBook.Model.OrdersAggregate
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Approved")]
        Approved,

        [EnumMember(Value = "Processing")]
        Processing,

        [EnumMember(Value = "Shipped")]
        Shipped,

        [EnumMember(Value = "Cancelled")]
        Cancelled
    }
}
