using Microsoft.AspNetCore.Identity;

namespace PROG6212POE.Models
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
