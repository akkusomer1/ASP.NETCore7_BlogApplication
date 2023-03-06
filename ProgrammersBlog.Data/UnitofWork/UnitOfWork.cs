using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork;
using ProgrammersBlog.Data.Context;
using ProgrammersBlog.Data.Repositories;

namespace ProgrammersBlog.Data.UnitofWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ProgrammersBlogContext _context;

        public UnitOfWork(ProgrammersBlogContext context)
        {
            _context = context;
        }

        public async ValueTask DisposeAsync()
        {
          await  _context.DisposeAsync();
        }

        public  int Commit()
        {
           return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        IGenericRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
        {
            return new GenericRepository<TEntity>(_context);
        }
    }
}
