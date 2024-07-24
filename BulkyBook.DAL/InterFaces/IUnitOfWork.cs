using BulkyBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DAL.InterFaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task<int> CompleteAsync();
    }
}
