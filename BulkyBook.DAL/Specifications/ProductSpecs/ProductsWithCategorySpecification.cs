using BulkyBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DAL.Specifications.ProductSpecs
{
    public class ProductsWithCategorySpecification : BaseSpecifications<Product>
    {
        public ProductsWithCategorySpecification()
        {
            Includes.Add(x => x.Category);
        }

        public ProductsWithCategorySpecification(int id) : base(x => x.Id == id)
        {
            Includes.Add(x => x.Category);
        }
    }
}
