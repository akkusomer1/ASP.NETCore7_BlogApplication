using ProgrammersBlog.Core.Abstract.Interfaces.IEntity;
using System.Linq.Expressions;

namespace ProgrammersBlog.Core.Abstract.İnterfaces.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity, new()
    {
        IQueryable<TEntity> GetAssQueryable();
        Task<TEntity> GetAsyncV2(IList<Expression<Func<TEntity, bool>>> pridicates,  IList<Expression<Func<TEntity, object>>> includeProperties);
        Task<IList<TEntity>> GetAllAsyncV2(IList<Expression<Func<TEntity, bool>>> pridicates, IList<Expression<Func<TEntity, object>>> includeProperties);
        Task<IList<TEntity>> SearchAsync(IList<Expression<Func<TEntity, bool>>> pridicates, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> pridicate, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> pridicate = null!, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> AddAsync(TEntity entity); 
        TEntity Update(TEntity entity);
        Task DeleteAsync(TEntity entity); 
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> pridicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> pridicate=null);
    }
}
