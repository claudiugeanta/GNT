using GNT.Domain.Models;
using GNT.Shared.Dtos.UserManagement;
using System.Linq.Expressions;

namespace GNT.Application.Mappings;

public static class UserMapping
{
    public static Expression<Func<User, UserDto>> DtoProjection
    {
        get
        {
            return d => new UserDto
            {
                Id = d.Id,
                Email = d.Email,
                FirstName = d.FirstName,
                LastName = d.LastName,
                IsBlocked = d.IsBlocked,
            };
        }
    }

    public static User CreateEntity(this CreateUserDto postModel)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = postModel.Email,
            FirstName = postModel.FirstName,
            LastName = postModel.LastName,
            IsBlocked = postModel.IsBlocked
        };
    }
}
