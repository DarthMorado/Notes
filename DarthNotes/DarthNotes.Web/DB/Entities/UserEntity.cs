using DarthNotes.Enums;

namespace DarthNotes.DB.Entities;

public class UserEntity : BaseEntity
{
    public string Username { get; set; }
    public UserTypeEnum UserType { get; set; }
    public bool IsPasswordAuthEnabled { get; set; }
    public string? AdditionalPasswordHash { get; set; }
    public string? Token { get; set; }
    public DateTime? TokenDate { get; set; }
}