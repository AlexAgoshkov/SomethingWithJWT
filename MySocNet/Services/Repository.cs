using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private MyDbContext _context;

        public Repository(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IList<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAllAsync()
            => await _context.Set<T>().CountAsync();

        public async Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().CountAsync(predicate);

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().FirstOrDefaultAsync(predicate); 
      
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);
       
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public async Task RemoveAsync(T entiry)
        {
            _context.Set<T>().Remove(entiry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
