namespace DarthNotes.DB.Entities;

public class NoteEntity : BaseEntity
{
    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public string? Content { get; set; }
    public string? Name { get; set; }
    public List<TagEntity> Tags { get; set; }
}