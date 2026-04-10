using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Shared.Dtos.Roles;
using GNT.Shared.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Account.Queries;

public class GetUserRoleListQuery : IRequest<IEnumerable<RoleDto>>
{
    public GetUserRoleListQuery(Guid userId) => UserId = userId;
    internal Guid UserId { get; set; }
}

internal class GetUserRoleListQueryHandler(
    IAppDbContext appDbContext,
    UserManager<User> userManager)
    : IRequestHandler<GetUserRoleListQuery, IEnumerable<RoleDto>>
{
    public async Task<IEnumerable<RoleDto>> Handle(GetUserRoleListQuery request, CancellationToken cancellationToken)
    {
        var user = await appDbContext.User
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new BusinessException(FailureCode.NotFound);

        var roleNames = await userManager.GetRolesAsync(user);

        return await appDbContext.Role
            .Where(r => roleNames.Contains(r.Name!))
            .Select(RoleMapping.DtoProjection)
            .ToListAsync(cancellationToken);
    }
}