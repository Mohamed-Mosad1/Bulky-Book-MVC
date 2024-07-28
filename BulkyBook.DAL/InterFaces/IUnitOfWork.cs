using BulkyBook.Model;

namespace BulkyBook.DAL.InterFaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseModel;

        Task<int> CompleteAsync();
    }
}
