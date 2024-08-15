using BulkyBook.DAL.Specifications;

namespace BulkyBook.DAL.InterFaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync<Type>(Type id);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(IBaseSpecifications<T> spec);
        Task<T?> GetWithSpecAsync(IBaseSpecifications<T> spec);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
