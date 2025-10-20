using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Services;
using Xunit;

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
        // Arrange
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

        // Act
        var claimId = await service.AddClaimAsync(claim);
        var addedClaim = await service.GetClaimAsync(claimId);

        // Assert
        Assert.NotNull(addedClaim);
        Assert.Equal("user456", addedClaim.UserId);
        Assert.Equal(5, addedClaim.HoursWorked);
    }

    [Fact]
    public async Task GetClaimAsync_ShouldReturnClaim()
    {
        // Arrange
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        // Act
        var claim = await service.GetClaimAsync(1);

        // Assert
        Assert.NotNull(claim);
        Assert.Equal("user123", claim.UserId);
        Assert.Equal("Pending", claim.Status);
    }

    [Fact]
    public async Task GetClaimsAsync_ShouldReturnAllClaims()
    {
        // Arrange
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        // Act
        var claims = await service.GetClaimsAsync();

        // Assert
        Assert.NotEmpty(claims);
        Assert.True(claims.Count >= 1);
    }

    [Fact]
    public async Task SetApprovalAsync_ShouldUpdateStatus()
    {
        // Arrange
        var context = await GetInMemoryDbContext();
        var service = new ClaimService(context);

        // Act
        await service.SetApprovalAsync(1, true);
        var updatedClaim = await service.GetClaimAsync(1);

        // Assert
        Assert.Equal("Approved", updatedClaim.Status);
    }

    [Fact]
    public async Task SubmitClaimAsync_ShouldSetCreatedAndStatus()
    {
        // Arrange
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

        // Act
        await service.SubmitClaimAsync(newClaim);
        var submittedClaim = await context.Claims.FirstOrDefaultAsync(c => c.UserId == "user789");

        // Assert
        Assert.NotNull(submittedClaim);
        Assert.Equal("Pending", submittedClaim.Status);
        Assert.True(submittedClaim.Created <= DateTime.UtcNow);
    }
}
