using DarthNotes.Core.Enums;

namespace DarthNotes.Db.Entities;

public class UserEntity : BaseEntity
{
    public string Username { get; set; }
    public UserTypeEnum UserType { get; set; }
}