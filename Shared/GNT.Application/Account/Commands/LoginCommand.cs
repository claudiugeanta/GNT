using GNT.Application.Account.Utils;
using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Dtos.UserManagement;
using GNT.Shared.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Account.Commands;

public class LoginCommand : IRequest<TokenDto>
{
    public LoginCommand(string email, string password, string securityCode)
    {
        Email = email;
        Password = password;
        SecurityCode = securityCode;
    }

    public string Email { get; set; }
    public string Password { get; set; }
    public string SecurityCode { get; set; }
}

public class LoginCommandHandler(
    IAppDbContext appDbContext,
    JwtService jwtService,
    UserManager<User> userManager)
    : IRequestHandler<LoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await appDbContext.User
            .Where(d => d.Email == request.Email)
            .Include(d => d.UserSecurityCodes
                .Where(s => s.Type == SecurityCodeTypes.TwoFactorAuthentication)
                .OrderByDescending(s => s.CreatedAt)
                .Take(1))
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessException(FailureCode.InvalidCredentials);

        var passwordHasher = new PasswordHasher<User>();
        dbUser.ValidatePassword(request.Password, passwordHasher);
        dbUser.ValidateStatus();

        // Incarca rolurile si permisiunile prin UserManager
        var roles = await userManager.GetRolesAsync(dbUser);
        var permissions = await appDbContext.Role
            .Where(r => roles.Contains(r.Name!))
            .Include(r => r.RolePermissions)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.PermissionId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var failureReason = dbUser.ValidateUserSecurityCode(request.SecurityCode);

        await appDbContext.SaveChangesAsync(cancellationToken);

        if (failureReason.HasValue)
            throw new BusinessException(failureReason.Value);

        var token = jwtService.GenerateToken(dbUser, permissions);
        return token;
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(d => d.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(d => d.Password).NotNull().NotEmpty();
        RuleFor(d => d.SecurityCode).NotNull().NotEmpty();
    }
}