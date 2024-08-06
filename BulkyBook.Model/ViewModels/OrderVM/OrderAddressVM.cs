using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Model.ViewModels.OrderVM
{
    public class OrderAddressVM
    {
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Street { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public string State { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
    }
}
