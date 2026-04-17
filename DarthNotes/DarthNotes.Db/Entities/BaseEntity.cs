using System.ComponentModel.DataAnnotations;

namespace DarthNotes.Db.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}