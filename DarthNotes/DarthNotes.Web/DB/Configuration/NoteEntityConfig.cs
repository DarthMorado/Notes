using DarthNotes.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarthNotes.DB.Configuration;

public class NoteEntityConfig: IEntityTypeConfiguration<NoteEntity>
{
    public void Configure(EntityTypeBuilder<NoteEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Tags)
            .WithMany(x => x.Notes);

    }
}