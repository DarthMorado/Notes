using DarthNotes.Enums;

namespace DarthNotes.DB.Entities;

public class UserEntity : BaseEntity
{
    public string Username { get; set; }
    public UserTypeEnum UserType { get; set; }
}