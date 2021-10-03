using Microsoft.EntityFrameworkCore;
using pw.Auth.DAL;
using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace pw.Auth.Services
{    
    public interface IBaseService<T> where T : class, IEntity, new()
    {
        Task<ICollection<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<ICollection<T>> GetAll();
        Task<int> Count();
        Task<T> GetSingle(Expression<Func<T, bool>> predicate);
        Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<T> Find(Expression<Func<T, bool>> match);
        Task<List<T>> FindList(Expression<Func<T, bool>> match);
        Task<T> Add(T entity);
        Task<int> AddRange(List<T> entities);
        Task<T> Update(T entity, object key);
        Task<int> Delete(T entity);
        Task<int> DeleteWhere(Expression<Func<T, bool>> predicate);
        void Commit();
    }

    public class BaseService<T> : IBaseService<T>
            where T : class, IEntity, new()
    {

        protected AuthDbContext _context;

        #region Properties
        public BaseService(AuthDbContext context)
        {
            _context = context;
        }
        #endregion

        public virtual async Task<ICollection<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
        public virtual async Task<ICollection<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<int> Count()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<T> Find(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(match);
        }

        public virtual async Task<List<T>> FindList(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync<T>();
        }

        public virtual async Task<T> Add(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<int> AddRange(List<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<T> Update(T entity, object key)
        {
            if (entity == null)
                return null;
            T exist = _context.Set<T>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }

            return exist;
        }
        public virtual async Task<int> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Set<T>().Remove(entity);
            }
            return await _context.SaveChangesAsync();
        }

        public virtual void Commit()
        {
            _context.SaveChanges();
        }
    }
}
