using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class Lecturers
    {

        [Key, Display(Name = "ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Hours Worked")]
        public double HoursWorked { get; set; } = 0;

        [Required]
        [Display(Name = "Hour Rate")]
        public double HourRate { get; set; } = 0;
    }
}
