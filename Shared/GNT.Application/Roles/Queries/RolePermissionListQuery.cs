using GNT.Application.Interfaces;
using GNT.Enums;
using Microsoft.EntityFrameworkCore;

namespace GNT.Administration.Application.Roles.Queries
{
    public class RolePermissionListQuery : IRequest<List<Permission>>
    {
        public RolePermissionListQuery(Guid roleId)
        {
            RoleId = roleId;
        }

        internal Guid RoleId { get; set; }
    }

    internal class RolePermissionListQueryHandler(IAppDbContext appDbContext) : IRequestHandler<RolePermissionListQuery, List<Permission>>
    {
        public async Task<List<Permission>> Handle(RolePermissionListQuery request, CancellationToken cancellationToken)
        {
            var permissions = await appDbContext.RolePermission
                .Where(d => d.RoleId == request.RoleId)
                .Select(d => d.PermissionId)
                .ToListAsync(cancellationToken);

            return permissions;
        }
    }
}
