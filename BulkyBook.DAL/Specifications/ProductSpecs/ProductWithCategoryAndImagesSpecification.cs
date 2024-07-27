using BulkyBook.Model;

namespace BulkyBook.DAL.Specifications.ProductSpecs
{
    public class ProductWithCategoryAndImagesSpecification : BaseSpecifications<Product>
    {
        public ProductWithCategoryAndImagesSpecification()
        {
            Includes.Add(x => x.Category);
            Includes.Add(x => x.ProductImages);
        }

        public ProductWithCategoryAndImagesSpecification(int id) : base(x => x.Id == id)
        {
            Includes.Add(x => x.Category);
            Includes.Add(x => x.ProductImages);
        }

    }
}
