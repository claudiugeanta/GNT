using GNT.Application.Account.Utils;
using GNT.Application.Interfaces;
using GNT.Common.Extensions;
using GNT.Domain.Models;
using GNT.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace GNT.Application.Account.Commands;

public class ChangePasswordCommand : IRequest<Unit>
{
    public ChangePasswordCommand(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }

    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

public class ChangePasswordCommandHandler(
    IAppDbContext appDbContext,
    ICurrentSession session,
    UserManager<User> userManager)
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = appDbContext.User
            .FirstOrDefault(d => d.Id == session.CurrentUser.Id)
            ?? throw new BusinessException(FailureCode.NotFound);

        var result = await userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new BusinessException(FailureCode.InvalidCredentials);

        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(d => d.NewPassword)
            .Must(d => d.IsAValidPasswordPattern())
            .WithState(d => FailureCode.InternalError);
    }
}