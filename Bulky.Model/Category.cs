using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Model
{
    public class Category
    {
        public int Id { get; set; }
        
        [DisplayName("Category Name")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters!")]
        public string Name { get; set; } = null!;

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100 only!")]
        public int DisplayOrder { get; set; }
    }
}
