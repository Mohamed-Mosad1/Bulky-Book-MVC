using System.Linq.Expressions;

namespace BulkyBook.DAL.Specifications
{
    public interface IBaseSpecifications<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludeStrings { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public bool AsNoTracking { get; set; }
        public Expression<Func<T, T>> Select { get; set; }
    }
}
