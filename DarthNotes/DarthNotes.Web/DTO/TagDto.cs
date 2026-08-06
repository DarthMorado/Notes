namespace DarthNotes.Web.DTO;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}