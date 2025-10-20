using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PROG6212POE.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        // Link to User who submitted the claim
        [Required]
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; } // navigation property

        [Required]
        [Display(Name = "Hours Worked")]
        [Range(0.5, 100)]
        public double HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(10, 1000)]
        public decimal HourlyRate { get; set; }

        [NotMapped]
        public decimal Total => (decimal)HoursWorked * HourlyRate;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime Created { get; set; } = DateTime.Now;

        // File Upload
        [Display(Name = "Document Path")]
        public string? FilePath { get; set; }

        [Display(Name = "Claim Status")]
        public string Status { get; set; } = "Pending"; // Pending, Verified, Approved, Rejected
    }
}
