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

        public int Take { get; set; }

        public int Skip { get; set; }

        public bool IsPagingEnabled { get; set; } = false;

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

        public void ApplyPagination(int skip, int take)
        {
            IsPagingEnabled = true;
            Skip = skip;
            Take = take;
        }

    }
}
