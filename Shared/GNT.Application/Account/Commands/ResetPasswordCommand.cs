using GNT.Application.Account.Utils;
using GNT.Application.Interfaces;
using GNT.Common.Extensions;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Dtos.UserManagement;
using GNT.Shared.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Account.Commands;

public class ResetPasswordCommand : IRequest<Unit>
{
    public ResetPasswordCommand(ResetPasswordDto model) => Model = model;
    public ResetPasswordDto Model { get; set; }
}

public class ResetPasswordCommandHandler(
    IAppDbContext appDbContext,
    UserManager<User> userManager)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await appDbContext.User
            .Where(d => d.Email == request.Model.Email)
            .Include(d => d.UserSecurityCodes
                .Where(s => s.Type == SecurityCodeTypes.ResetPassword)
                .OrderByDescending(s => s.CreatedAt)
                .Take(1))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessException(FailureCode.NotFound);

        dbUser.ValidateStatus();

        var failureReason = dbUser.ValidateUserSecurityCode(request.Model.SecurityCode);

        await appDbContext.SaveChangesAsync(cancellationToken);

        if (failureReason.HasValue)
            throw new BusinessException(failureReason.Value);

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(dbUser);
        var result = await userManager.ResetPasswordAsync(dbUser, resetToken, request.Model.NewPassword);

        if (!result.Succeeded)
            throw new BusinessException(FailureCode.InternalError);

        dbUser.UserSecurityCodes.Single().SuccessfullyUsed = true;
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(d => d.Model.NewPassword)
            .Matches(StringExtensions.PasswordValidationExpression)
            .WithState(d => FailureCode.InvalidPasswordPattern);
    }
}