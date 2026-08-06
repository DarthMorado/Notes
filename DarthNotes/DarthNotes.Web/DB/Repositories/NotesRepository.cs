using System.Linq.Expressions;
using DarthNotes.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarthNotes.DB.Repositories;

public interface INotesRepository : IBaseRepository<NoteEntity>
{
    
}

public class NotesRepository : BaseRepository<NoteEntity>, INotesRepository
{
    public NotesRepository(Database context) : base(context)
    {
    }

    public override async Task<List<NoteEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.Tags)
            .ToListAsync();
    }
    
    public override async Task<NoteEntity> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(x => x.Tags)
            .FirstAsync(x => x.Id == id);
    }
    
    public override async Task<List<NoteEntity>> FindAsync(Expression<Func<NoteEntity, bool>> predicate, bool noTracking = false)
    {
        var query = _dbSet.Include(x=> x.Tags).Where(predicate);
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query.ToListAsync();
    }
}