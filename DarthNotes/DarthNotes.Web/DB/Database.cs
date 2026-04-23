using DarthNotes.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarthNotes.DB;

public class Database : DbContext
{
    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }
    
    public DbSet<UserEntity> Users { get; set; }
}