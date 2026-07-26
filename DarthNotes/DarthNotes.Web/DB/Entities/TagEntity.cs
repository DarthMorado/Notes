namespace DarthNotes.DB.Entities;

public class TagEntity : BaseEntity
{
    public string Name { get; set; }
    public List<NoteEntity> Notes { get; set; }
}