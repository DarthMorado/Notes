using DarthNotes.Enums;

namespace DarthNotes.Web.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserTypeEnum UserType { get; set; }
    public bool IsPasswordAuthEnabled { get; set; }
    public string AdditionalPasswordHash { get; set; }
}