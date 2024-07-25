using BulkyBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DAL.Specifications.ProductSpecs
{
    public class ProductWithCategoryAndImagesSpecification : BaseSpecifications<Product>
    {
        public ProductWithCategoryAndImagesSpecification()
        {
            Includes.Add(x => x.Category);
        }

        public ProductWithCategoryAndImagesSpecification(int id) : base(x => x.Id == id)
        {
            Includes.Add(x => x.Category);
            Includes.Add(x => x.ProductImages);
        }

    }
}
