using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task<IEnumerable<T>> FindAllAsync(
         Expression<Func<T, bool>>? predicate = null,
         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        );
        void HardDelete(T entity);
        void SoftDelete(T entity);
        // Added methods needed by our manager classes
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Delete(T entity);
    }
}
