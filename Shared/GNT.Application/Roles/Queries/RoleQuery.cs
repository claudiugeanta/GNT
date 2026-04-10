using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Dtos.Roles;
using Microsoft.EntityFrameworkCore;

namespace GNT.Administration.Application.Organizations.Queries
{
    public class RoleQuery : IRequest<RoleDto>
    {
        public RoleQuery(Guid id)
        {
            Id = id;
        }

        internal Guid Id { get; set; }
    }

    internal class RoleQueryHandler(IAppDbContext appDbContext) : IRequestHandler<RoleQuery, RoleDto>
    {
        public async Task<RoleDto> Handle(RoleQuery request, CancellationToken cancellationToken)
        {
            var role = await appDbContext.Role.Where(d => d.Id == request.Id)
                .Select(RoleMapping.DtoProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return role;
        }
    }
}
