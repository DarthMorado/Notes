namespace DarthNotes.Web.Models;

public class BaseNoteModel
{
    public int? Id { get; set; }
    public ModelMode Mode { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
    public List<string> Tags { get; set; }
}