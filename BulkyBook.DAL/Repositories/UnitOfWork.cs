using BulkyBook.DAL.Data;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model;
using System.Collections;

namespace BulkyBook.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseModel
        {
            var key = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(key))
            {
                var repositoryInstance = new GenericRepository<TEntity>(_dbContext);

                _repositories.Add(key, repositoryInstance);
            }

            return (IGenericRepository<TEntity>)_repositories[key];
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

    }
}
