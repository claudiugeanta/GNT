using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Dtos.Roles;
using System.Linq.Expressions;

namespace GNT.Application.Mappings
{
    public static class RoleMapping
    {
        public static Expression<Func<Role, RoleDto>> DtoProjection
        {
            get
            {
                return d => new RoleDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    IsDefault = d.IsDefaultRole,
                };
            }
        }

        public static Role CreateEntity(this CreateRoleDto postModel)
        {
            return new Role
            {
                Name = postModel.Name,
                Description = postModel.Description,
            };
        }
    }
}
