﻿namespace BulkyBook.Model
{
    public class Company : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Street { get; set; }
        public string? State { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
