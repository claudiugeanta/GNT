using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Shared.Dtos.UserManagement;
using GNT.Shared.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Account.Commands;

public class ManageUserRolesCommand : IRequest<Unit>
{
    public ManageUserRolesCommand(Guid userId, ManageUserRolesDto postModel)
    {
        UserId = userId;
        PostModel = postModel;
    }

    internal Guid UserId { get; set; }
    internal ManageUserRolesDto PostModel { get; set; }
}

internal class ManageUserRolesCommandHandler(
    IAppDbContext appDbContext,
    UserManager<User> userManager)
    : IRequestHandler<ManageUserRolesCommand, Unit>
{
    public async Task<Unit> Handle(ManageUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await appDbContext.User
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new BusinessException(FailureCode.NotFound);

        // Rolurile sunt identificate prin nume in Identity
        var rolesToAdd = await appDbContext.Role
            .Where(r => request.PostModel.RolesToAdd.Contains(r.Id))
            .Select(r => r.Name!)
            .ToListAsync(cancellationToken);

        var rolesToRemove = await appDbContext.Role
            .Where(r => request.PostModel.RolesToRemove.Contains(r.Id))
            .Select(r => r.Name!)
            .ToListAsync(cancellationToken);

        if (rolesToAdd.Any())
            await userManager.AddToRolesAsync(user, rolesToAdd);

        if (rolesToRemove.Any())
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);

        return Unit.Value;
    }
}