using System.ComponentModel.DataAnnotations;
using PROG6212POE.Models;

public class Admin
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    // Additional admin-specific fields
}
