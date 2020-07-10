using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    //public class MyUserService : IRepository<User>
    //{
    //    private MyDbContext _context;
    //    public MyUserService(MyDbContext myDbContext)
    //    {
    //        _context = myDbContext;
    //    }

    //    public async Task AddAsync(User entity)
    //    {
    //        await _context.Set<User>().AddAsync(entity);
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task<int> CountAllAsync() 
    //        => await _context.Set<User>().CountAsync();

    //    public Task<int> CountWhereAsync(Expression<Func<User, bool>> predicate)
    //        => _context.Set<User>().CountAsync(predicate);

    //    public async Task<User> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    //        => await _context.Set<User>().FirstOrDefaultAsync(predicate);
        
    //    public async Task<IEnumerable<User>> GetAllAsync()
    //    {
    //        return await _context.Set<User>().ToListAsync();
    //    }

    //    public async Task<User> GetByIdAsync(int id) 
    //        => await _context.Set<User>().FindAsync(id);
       
    //    public IQueryable<User> GetWhereAsync(Expression<Func<User, bool>> predicate)
    //    {
    //        return _context.Users.Where(predicate);
    //    }

    //    public async Task RemoveAsync(User entity)
    //    {
    //        _context.Set<User>().Remove(entity);
    //          await _context.SaveChangesAsync();
    //    }

    //    public async Task RemoveAsyncById(int id)
    //    {
    //        var user = await _context.Users.FindAsync(id);
    //        _context.Users.Remove(user);
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task UpdateAsync(User entity)
    //    {
    //        _context.Entry(entity).State = EntityState.Modified;
    //        await _context.SaveChangesAsync();
    //    }
    //}
}
