using System.Linq.Expressions;

namespace BulkyBook.DAL.Specifications
{
    public class BaseSpecifications<T> : IBaseSpecifications<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; set; } = null!;

        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludeStrings { get; set; } = new List<string>();

        public Expression<Func<T, object>> OrderBy { get; set; } = null!;

        public Expression<Func<T, object>> OrderByDescending { get; set; } = null!;

        public Expression<Func<T, T>> Select { get; set; } = null!;

        public bool AsNoTracking { get; set; }

        public BaseSpecifications() { }

        public BaseSpecifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void AddOrderBy(Expression<Func<T, object>> OrderByExpression)
        {
            OrderBy = OrderByExpression;
        }

        public void AddOrderByDesc(Expression<Func<T, object>> OrderByDescExpression)
        {
            OrderByDescending = OrderByDescExpression;
        }

        public void ApplyNoTracking()
        {
            AsNoTracking = true;
        }

        public void AddSelect(Expression<Func<T, T>> selectExpression)
        {
            Select = selectExpression;
        }


    }
}
