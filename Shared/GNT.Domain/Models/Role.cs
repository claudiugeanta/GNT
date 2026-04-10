using GNT.Domain.BaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GNT.Domain.Models;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
        RolePermissions = new HashSet<RolePermission>();
    }

    public string Description { get; set; }

    public bool IsDefaultRole { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
    }
}
