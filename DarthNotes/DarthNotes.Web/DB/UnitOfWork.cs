using Microsoft.EntityFrameworkCore.Storage;

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
    private IDbContextTransaction _transaction;
    private bool _isCompleted = false;
    
    public Scope(Database db)
    {
        _db = db;
        _transaction = _db.Database.BeginTransaction();
    }

    public async Task Complete(CancellationToken token = default)
    {
        await _db.SaveChangesAsync(token);
        await _transaction.CommitAsync(token);
        _isCompleted = true;
    }

    public void Rollback()
    { 
        _db.ChangeTracker.Clear(); 
        _transaction.Rollback();
    }

    public void Dispose()
    {
        if (!_isCompleted)
        {
            Rollback();    
        }
        _transaction.Dispose();
    }

}