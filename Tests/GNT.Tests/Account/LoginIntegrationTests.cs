using FluentAssertions;
using GNT.Application.Account.Utils;
using GNT.Infrastructure.Context;
using GNT.Shared.Dtos.UserManagement;
using GNT.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Crypto.Generators;
using System.Net;
using System.Net.Http.Json;

namespace GNT.Tests.Account;

public class LoginIntegrationTests : IClassFixture<GntWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly GntWebApplicationFactory _factory;

    public LoginIntegrationTests(GntWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns400()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@test.com",
            Password = "WrongPassword123!",
            SecurityCode = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authentication/log-in", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var securityCode = "123456";

        var user = new GNT.Domain.Models.User
        {
            Id = Guid.NewGuid(),
            Email = "test2@test.com",
            FirstName = "Test",
            LastName = "User",
            Password = AccountService.HashPassword("Test123!@#"),
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsBlocked = false
        };

        user.UserSecurityCodes.Add(new GNT.Domain.Models.UserSecurityCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = securityCode,
            Type = GNT.Enums.SecurityCodeTypes.TwoFactorAuthentication,
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMinutes(5),
            SuccessfullyUsed = false,
            FailedAttempts = 0
        });

        db.User.Add(user);
        await db.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Email = "test2@test.com",
            Password = "Test123!@#",
            SecurityCode = securityCode
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authentication/log-in", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();
        token.Should().NotBeNull();
        token!.Value.Should().NotBeNullOrEmpty();
    }
}