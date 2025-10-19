using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public class ClaimService : IClaimService
    {

        private readonly ClaimsDBContext _context;

        public ClaimService(ClaimsDBContext claim)
        {
            this._context = claim;
        }
        public int AddClaim(Claims claim)
        {
            _context.ClaimDB.Add(claim);
            return claim.Id;
        }

        public Claims GetClaim(int id)
        {
            var book = _context.ClaimDB.FirstOrDefault(x => x.Id == id);
            return book!;
        }

        public List<Claims> GetClaims()
        {
            return _context.ClaimDB.ToList();
        }

        public void SetApproval(int Id, bool status)
        {
            var book = _context.ClaimDB.FirstOrDefault(x => x.Id == Id);
            if (book != null)
            {
                book.Approval = status;
                _context.SaveChanges();
            }
        }

        public void submitClaim(Claims claim) 
        {
        
        
        }
    }
}
