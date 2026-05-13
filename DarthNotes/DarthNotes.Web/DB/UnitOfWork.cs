namespace DarthNotes.DB;

public interface IUnitOfWork
{
    Scope CreateScope();
    Task SaveChangesAsync(CancellationToken token = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly Database _db;

    public UnitOfWork(Database db)
    {
        _db = db;
    }

    public Scope CreateScope()
    {
        return new Scope(_db);
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        await _db.SaveChangesAsync(token);
    }
}

public class Scope : IDisposable
{
    private readonly Database _db;
    
    public Scope(Database db)
    {
        _db = db;
        _db.Database.BeginTransaction();
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        await _db.SaveChangesAsync(token);
    }

    public void RollbackAsync()
    { 
        _db.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        RollbackAsync();
    }
}