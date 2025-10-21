using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;
using Claim = PROG6212POE.Models.Claim;

namespace PROG6212POE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ClaimsDBContext _context;

        public ClaimService(ClaimsDBContext context)
        {
            _context = context;
        }

        // Adds a claim and returns the ID
        public async Task<int> AddClaimAsync(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
            return claim.Id;
        }

        public async Task<Claim?> GetClaimAsync(int id)
        {
            return await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Claim>> GetClaimsAsync()
        {
            return await _context.Claims.ToListAsync();
        }

        public async Task SetApprovalAsync(int id, bool isApproved)
        {
            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = isApproved ? "Approved" : "Rejected";
                await _context.SaveChangesAsync();
            }
        }

        public async Task SubmitClaimAsync(Claim claim)
        {
            claim.Status = "Pending";
            claim.Created = DateTime.UtcNow;  // should be UtcNow to match test assertion
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
        }
    }
}