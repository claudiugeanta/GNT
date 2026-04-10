using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;

namespace GNT.Domain.Models;

public class User : IdentityUser<Guid>
{
    public User()
    {
        UserSecurityCodes = new HashSet<UserSecurityCode>();
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public bool IsBlocked { get; set; }
    public DateTime? UnblockDate { get; set; }


    public virtual ICollection<UserSecurityCode> UserSecurityCodes { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
    }
}