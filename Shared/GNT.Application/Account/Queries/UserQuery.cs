using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Shared.Dtos.UserManagement;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Account.Queries;

public class UserQuery : IRequest<UserDto>
{
    public UserQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}

public class UserQueryHandler(IAppDbContext appDbContext, ICurrentSession session) : IRequestHandler<UserQuery, UserDto>
{
    public async Task<UserDto> Handle(UserQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            request.Id = session.CurrentUser.Id;
        }

        var user = await appDbContext.User
            .Where(d=>d.Id == request.Id)
            .Select(UserMapping.DtoProjection)
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}
