using FluentAssertions;
using GNT.Application.Account.Utils;
using GNT.Domain.Models;
using GNT.Infrastructure.Context;
using GNT.Shared.Dtos.UserManagement;
using GNT.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
        var loginDto = new LoginDto
        {
            Email = "nonexistent@test.com",
            Password = "WrongPassword123!",
            SecurityCode = "123456"
        };

        var response = await _client.PostAsJsonAsync("/api/authentication/log-in", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var securityCode = "123456";
        var password = "Test123!@#";

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "test2@test.com",
            Email = "test2@test.com",
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true,
            IsBlocked = false
        };

        await userManager.CreateAsync(user, password);

        // Adauga security code direct in DB
        db.UserSecurityCode.Add(new UserSecurityCode
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

        await db.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Email = "test2@test.com",
            Password = password,
            SecurityCode = securityCode
        };

        var response = await _client.PostAsJsonAsync("/api/authentication/log-in", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();
        token.Should().NotBeNull();
        token!.Value.Should().NotBeNullOrEmpty();
    }
}