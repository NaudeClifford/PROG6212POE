using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models.ViewModels
{
    public class ClaimViewModel
    {
        [Required]
        [Range(0.5, 180, ErrorMessage = "You cannot claim more than 180 hours per month.")]
        public double HoursWorked { get; set; }

        public decimal HourlyRate { get; set; }

        public string? Notes { get; set; }

        [Required]
        [AllowedExtensions(new[] { ".pdf", ".docx", ".xlsx" })]
        [Display(Name = "Upload Document")]
        public IFormFile Document { get; set; } = null!;

        public decimal Total => (decimal)HoursWorked * HourlyRate;
    }
}
