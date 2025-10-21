using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Services;

public class ClaimServiceTests
{
    private async Task<ClaimsDBContext> GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ClaimsDBContext>()
            .UseInMemoryDatabase(databaseName: "ClaimTestDb_" + Guid.NewGuid())
            .Options;

        var context = new ClaimsDBContext(options);

        // Seed one claim
        if (!context.Claims.Any())
        {
            context.Claims.Add(new Claim
            {
                Id = 1,
                UserId = "user123",
                HoursWorked = 10,
                HourlyRate = 150,
                Notes = "Initial claim",
                FilePath = "claim1.pdf",
                Status = "Pending",
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        return context;
    }

    [Fact]
    public async Task AddClaimAsync_ShouldAddClaim()
    {
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        var claim = new Claim
        {
            UserId = "user456",
            HoursWorked = 5,
            HourlyRate = 200,
            Notes = "New claim",
            FilePath = "claim2.pdf"
        };

        var claimId = await service.AddClaimAsync(claim);
        var addedClaim = await service.GetClaimAsync(claimId);

        Assert.NotNull(addedClaim);
        Assert.Equal("user456", addedClaim.UserId);
        Assert.Equal(5, addedClaim.HoursWorked);
    }

    [Fact]
    public async Task GetClaimAsync_ShouldReturnClaim()
    {
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        var claim = await service.GetClaimAsync(1);

        Assert.NotNull(claim);
        Assert.Equal("user123", claim.UserId);
        Assert.Equal("Pending", claim.Status);
    }

    [Fact]
    public async Task GetClaimsAsync_ShouldReturnAllClaims()
    {
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        var claims = await service.GetClaimsAsync();

        Assert.NotEmpty(claims);
        Assert.True(claims.Count >= 1);
    }

    [Fact]
    public async Task SetApprovalAsync_ShouldUpdateStatus()
    {
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        await service.SetApprovalAsync(1, true);
        var updatedClaim = await service.GetClaimAsync(1);

        Assert.Equal("Approved", updatedClaim?.Status);
    }

    [Fact]
    public async Task SubmitClaimAsync_ShouldSetCreatedAndStatus()
    {
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);
        var newClaim = new Claim
        {
            UserId = "user789",
            HoursWorked = 8,
            HourlyRate = 120,
            Notes = "Submission test",
            FilePath = "claim3.pdf"
        };

        await service.SubmitClaimAsync(newClaim);
        var submittedClaim = await context.Claims.FirstOrDefaultAsync(c => c.UserId == "user789");
        
        Assert.NotNull(submittedClaim);
        Assert.Equal("Pending", submittedClaim.Status);
        Assert.True(submittedClaim.Created <= DateTime.UtcNow);
    }
}
