using DarthNotes.Enums;

namespace DarthNotes.Web.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserTypeEnum UserType { get; set; }
}