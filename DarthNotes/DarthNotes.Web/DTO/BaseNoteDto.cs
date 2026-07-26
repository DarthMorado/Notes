using DarthNotes.Web.DTO;

namespace DarthNotes.Web.Models;

public class BaseNoteDto
{
    public int? Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
    public List<TagDto> Tags { get; set; }
}