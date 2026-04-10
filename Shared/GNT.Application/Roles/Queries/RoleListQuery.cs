using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Dtos.Roles;
using Microsoft.EntityFrameworkCore;

namespace GNT.Administration.Application.Roles.Queries
{
    public class RoleListQuery : IRequest<IEnumerable<RoleDto>>
    {
        public RoleListQuery()
        {
        }
    }

    internal class RoleListQueryHandler(IAppDbContext appDbContext) : IRequestHandler<RoleListQuery, IEnumerable<RoleDto>>
    {
        public async Task<IEnumerable<RoleDto>> Handle(RoleListQuery request, CancellationToken cancellationToken)
        {
            var roles = await appDbContext.Role
                .OrderBy(d => !d.IsDefaultRole)
                .ThenBy(d => d.Name)
                .Select(RoleMapping.DtoProjection)
                .ToListAsync(cancellationToken);

            return roles;
        }
    }
}
