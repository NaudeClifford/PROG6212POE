using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212POE.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public User? User { get; set; }

        [Required, Display(Name = "Hours Worked"), Range(0.5, 180,
            ErrorMessage = "Hours worked cannot exceed 180 hours per month.")]
        public double HoursWorked { get; set; }

        [Required, Display(Name = "Hourly Rate"), Range(10, 1000)]
        public decimal HourlyRate { get; set; }

        [NotMapped]
        public decimal Total => (decimal)HoursWorked * HourlyRate;

        public string? Notes { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public string? FilePath { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
