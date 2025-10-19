using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class Claims
    {
        [Key, Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Created")]
        public DateTime Created { get; set; }

        [Display(Name = "Approval")]
        public Boolean Approval { get; set; }
        

    }
}
