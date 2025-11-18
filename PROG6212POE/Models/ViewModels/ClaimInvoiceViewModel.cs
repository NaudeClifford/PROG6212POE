namespace PROG6212POE.Models.ViewModels
{
    public class ClaimInvoiceViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<Claim> ApprovedClaims { get; set; } = new();
        public decimal TotalAmount => ApprovedClaims.Sum(c => c.Total);
        public DateTime GeneratedOn { get; set; } = DateTime.Now;
    }

}
