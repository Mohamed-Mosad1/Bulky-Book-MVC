// Ignore Spelling: Dto App

using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Model.ViewModels
{
    public class OrderVM
    {
        public string? AppUserId { get; set; }
        public string? CartId { get; set; }

        [Required]
        public OrderAddressVM OrderAddress { get; set; } = null!;
    }
}
