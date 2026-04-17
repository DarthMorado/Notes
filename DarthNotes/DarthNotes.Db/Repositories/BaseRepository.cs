using System.Linq.Expressions;
using DarthNotes.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarthNotes.Db.Repositories;

public interface IBaseRepository<T>
    where T : BaseEntity
{
    public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    public Task AddAsync(T entity);
    public Task SaveChangesAsync();
}

public class BaseRepository<T> : IBaseRepository<T>
    where T : BaseEntity
{
    protected readonly Database _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(Database context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await Task.CompletedTask;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public virtual async Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}