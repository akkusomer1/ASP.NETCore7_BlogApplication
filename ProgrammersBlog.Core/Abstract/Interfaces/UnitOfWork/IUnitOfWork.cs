using ProgrammersBlog.Core.Abstract.Interfaces.IEntity;
using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>()  where TEntity : class, IEntity, new();
        int Commit();
        Task<int> CommitAsync();
    }
}
