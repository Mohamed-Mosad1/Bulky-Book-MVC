using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Model.ViewModels
{
    public class ProductImageVM
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;

        public int ProductId { get; set; }
    }
}
