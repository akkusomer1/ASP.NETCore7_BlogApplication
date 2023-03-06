using LinqKit;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Abstract.Interfaces.IEntity;
using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Data.Context;
using System;
using System.Linq.Expressions;

namespace ProgrammersBlog.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity, new()
    {
        protected readonly ProgrammersBlogContext _context;

        public GenericRepository(ProgrammersBlogContext context)
        {
            _context = context;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> pridicate)
        {
            return await _context.Set<TEntity>().AnyAsync(pridicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> pridicate=null)
        {
            return pridicate == null ? await _context.Set<TEntity>().CountAsync() : await _context.Set<TEntity>().CountAsync(pridicate);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => { _context.Set<TEntity>().Remove(entity); });
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> pridicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (pridicate != null)
            {
                query = query.Where(pridicate);
            }

            if (includeProperties.Any())
            {
                foreach (var item in includeProperties)
                {
                    query = query.Include(item);
                }
            }
            return await query.AsNoTracking().ToListAsync();
        }

       
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> pridicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (includeProperties.Any())
            {
                foreach (var item in includeProperties)
                {
                    query = query.Include(item);
                }
            }
            return await query.AsNoTracking().SingleOrDefaultAsync(pridicate);
        }
     
        public TEntity Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            return entity;
        }

        public async Task<IList<TEntity>> SearchAsync(IList<Expression<Func<TEntity, bool>>> predicates, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            {
                var predicatesChain = PredicateBuilder.New<TEntity>();
                foreach (var pridicate in predicates)
                {
                    predicatesChain.Or(pridicate);
                }
                query = query.Where(predicatesChain);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.ToListAsync();
        }

        public IQueryable<TEntity> GetAssQueryable()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public async Task<TEntity> GetAsyncV2(IList<Expression<Func<TEntity, bool>>> pridicates, IList<Expression<Func<TEntity, object>>> includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (pridicates!=null&&pridicates.Any())
            {
                foreach (var pridicate in pridicates)
                {
                    query.Where(pridicate);
                }
            }
            
            if (includeProperties!=null && includeProperties.Any())
            {
                foreach (var item in includeProperties)
                {
                    query = query.Include(item);
                }
            }
            return await query.AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<IList<TEntity>> GetAllAsyncV2(IList<Expression<Func<TEntity, bool>>> pridicates, IList<Expression<Func<TEntity, object>>> includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (pridicates != null && pridicates.Any())
            {
                foreach (var pridicate in pridicates)
                {
                    query.Where(pridicate);
                }
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var item in includeProperties)
                {
                    query = query.Include(item);
                }
            }
            return await query.AsNoTracking().ToListAsync();
        }
    }
}
