using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class FileUploadModel
    {

        [Required]
        [Display(Name = "Document")]
        public IFormFile? Document { get; set; }

        [Display(Name = "Claim ID")]
        public string? ClaimId { get; set; }

        [Display(Name = "Lecturer Name")]
        public string? LecturerName { get; set; }
    }
}
