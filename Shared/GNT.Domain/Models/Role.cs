using GNT.Domain.BaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GNT.Domain.Models;

public class Role : BaseEntity
{
    public Role()
    {
        RolePermissions = new HashSet<RolePermission>();
        UserRoles = new HashSet<UserRole>();
    }

    public string Name { get; set; }
    public string Description { get; set; }

    public bool IsDefaultRole { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.ConfigureBase();
    }
}
