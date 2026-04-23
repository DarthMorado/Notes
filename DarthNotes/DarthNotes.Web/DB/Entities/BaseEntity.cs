using System.ComponentModel.DataAnnotations;

namespace DarthNotes.DB.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}