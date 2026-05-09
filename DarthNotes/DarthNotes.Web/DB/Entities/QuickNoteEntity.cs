namespace DarthNotes.DB.Entities;

public class QuickNoteEntity : BaseEntity
{
    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
}