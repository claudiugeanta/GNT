using GNT.Domain.BaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace GNT.Domain.Models;

public class User : BaseEntity
{
    public User()
    {
        UserSecurityCodes = new HashSet<UserSecurityCode>();
    }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Password { get; set; }

    public bool IsBlocked { get; set; }
    public DateTime? UnblockDate { get; set; }


    public virtual ICollection<UserSecurityCode> UserSecurityCodes { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ConfigureBase();
    }
}