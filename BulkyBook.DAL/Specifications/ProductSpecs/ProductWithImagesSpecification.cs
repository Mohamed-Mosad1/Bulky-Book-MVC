using BulkyBook.Model;

namespace BulkyBook.DAL.Specifications.ProductSpecs
{
    public class ProductWithImagesSpecification : BaseSpecifications<Product>
    {
        public ProductWithImagesSpecification(int id)
            : base(x => x.Id == id)
        {
            AddInclude(x => x.ProductImages);
        }
    }
}
