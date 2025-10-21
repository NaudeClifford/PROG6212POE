using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public interface IClaimService
    {
        Task<int> AddClaimAsync(Claim claim);

        Task<Claim?> GetClaimAsync(int id);

        Task<List<Claim>> GetClaimsAsync();

        Task SetApprovalAsync(int id, bool isApproved);

        Task SubmitClaimAsync(Claim claim);
    }
}
