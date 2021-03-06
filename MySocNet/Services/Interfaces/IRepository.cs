﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task RemoveAsync(T entiry);

        Task RemoveRangeAsync(IEnumerable<T> entiry);

        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate);

        Task<int> CountAllAsync();

        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
    }
}
