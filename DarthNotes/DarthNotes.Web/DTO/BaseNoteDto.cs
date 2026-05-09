namespace DarthNotes.Web.Models;

public class BaseNoteDto
{
    public int? Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
}