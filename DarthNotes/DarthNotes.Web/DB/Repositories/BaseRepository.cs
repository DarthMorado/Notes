using System.Linq.Expressions;
using DarthNotes.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarthNotes.DB.Repositories;

public interface IBaseRepository<T>
    where T : BaseEntity
{
    public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, bool noTracking = false);
    public Task AddAsync(T entity);
    public Task SaveChangesAsync();
    public Task<T> GetByIdAsync(int id);
    public Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
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

    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, bool noTracking = false)
    {
        var query = _dbSet.Where(predicate);
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        _dbSet.Add(entity);
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

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}