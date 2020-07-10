using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task RemoveAsyncById(int id);

        Task RemoveAsync(T entiry);

        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetWhereAsync(Expression<Func<T, bool>> predicate);

        Task<int> CountAllAsync();

        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
    }
}
