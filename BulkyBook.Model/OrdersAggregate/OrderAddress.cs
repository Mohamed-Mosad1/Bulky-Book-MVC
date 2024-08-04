namespace BulkyBook.Model.OrdersAggregate
{
    public class OrderAddress
    {
        public OrderAddress() { }

        public OrderAddress(string fullName, string city, string street, string state, string phoneNumber)
        {
            FullName = fullName;
            Street = street;
            City = city;
            State = state;
            PhoneNumber = phoneNumber;
        }

        public string FullName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
