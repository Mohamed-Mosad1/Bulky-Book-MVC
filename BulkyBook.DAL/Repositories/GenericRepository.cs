using BulkyBook.DAL.Data;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications;
using BulkyBook.Model;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
        {
            _dbContext.Add(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(IBaseSpecifications<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public Task<T?> GetWithSpecAsync(IBaseSpecifications<T> spec)
        {
            return ApplySpecification(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecification(IBaseSpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbContext.FindAsync<T>(id);
        }


    }
}
