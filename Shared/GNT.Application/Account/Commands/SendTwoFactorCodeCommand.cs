using GNT.Application.Account.Utils;
using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GNT.Application.Account.Commands;

public class SendTwoFactorCodeCommand : IRequest<Unit>
{
    public SendTwoFactorCodeCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; set; }
    public string Password { get; set; }
}

public class SendTwoFactorCodeCommandHandler(
    IAppDbContext appDbContext,
    IEmailService emailService,
    IPasswordHasher<User> passwordHasher)
    : IRequestHandler<SendTwoFactorCodeCommand, Unit>
{
    public async Task<Unit> Handle(SendTwoFactorCodeCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await appDbContext.User
            .FirstOrDefaultAsync(d => d.Email == request.Email, cancellationToken)
            ?? throw new BusinessException(FailureCode.InvalidCredentials);

        dbUser.ValidatePassword(request.Password, passwordHasher);
        dbUser.ValidateStatus();
        dbUser.GenerateUserSecurityCode(SecurityCodeTypes.TwoFactorAuthentication);

        await appDbContext.SaveChangesAsync(cancellationToken);
        await SendTwoFactorCodeEmail(dbUser);

        return Unit.Value;
    }

    private Task SendTwoFactorCodeEmail(User user)
    {
        var code = user.UserSecurityCodes.First().Code;
        var expiresAt = user.UserSecurityCodes.First().ExpiresAt.ToString("dd/MM/yyyy HH:mm");

        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\EmailTemplates";
        var template = File.ReadAllText(assemblyPath + "\\SendTwoFactorCode.html");

        template = template
            .Replace("{SecurityCode}", code)
            .Replace("{ExpiresAt}", expiresAt)
            .Replace("{FirstName}", user.FirstName);

        emailService.QuickSendAsync("GNT Login Code", template, user.Email!);

        return Task.CompletedTask;
    }
}