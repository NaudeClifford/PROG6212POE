using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public interface IClaimService
    {
        public int AddClaim(Claims claim);

        public Claims GetClaim(int id);

        public List<Claims> GetClaims();

        public void SetApproval(int Id, bool status);

        public void submitClaim(Claims claim);
    }

    
}
