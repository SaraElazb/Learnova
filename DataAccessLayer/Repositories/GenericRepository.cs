using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ELearningDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ELearningDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public async Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
                {
                    IQueryable<T> query = _dbSet;

                    if (predicate != null)
                    {
                        query = query.Where(predicate);
                    }

                    if (include != null)
                    {
                        query = include(query);
                    }

                    return await query.ToListAsync();
                }
        public async Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
                {
                    IQueryable<T> query = _dbSet;

                    if (include != null)
                    {
                        query = include(query);
                    }

                    return await query.FirstOrDefaultAsync(predicate);
                }

        public void HardDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SoftDelete(T entity)
        {
            var isActiveProp = entity.GetType().GetProperty("IsActive");
            isActiveProp.SetValue(entity, false);
        }
    }

}
