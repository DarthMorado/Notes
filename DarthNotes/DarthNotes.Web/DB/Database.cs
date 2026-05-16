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
    public DbSet<NoteEntity> QuickNotes { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
}