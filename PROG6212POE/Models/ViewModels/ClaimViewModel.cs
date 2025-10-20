using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models.ViewModels
{
    public class ClaimViewModel
    {
        [Required]
        [Range(0.5, 1000)]
        public double HoursWorked { get; set; }

        [Required]
        [Range(10, 1000)]
        public decimal HourlyRate { get; set; }

        public string? Notes { get; set; }

        [Required, AllowedExtensions(new[] { ".pdf", ".docx", ".xlsx" })]
        [Display(Name = "Upload Document")]
        public IFormFile Document { get; set; } = null!;
    }
}
