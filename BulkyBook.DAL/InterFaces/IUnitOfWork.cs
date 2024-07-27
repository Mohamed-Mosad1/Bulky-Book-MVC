namespace BulkyBook.DAL.InterFaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task<int> CompleteAsync();
    }
}
