using GNT.Application.Interfaces;

namespace GNT.Administration.Application.Roles.Commands
{
    public class DeleteRoleCommand : IRequest<Unit>
    {
        public DeleteRoleCommand(Guid roleId)
        {
            RoleId = roleId;
        }

        internal Guid RoleId { get; set; }
    }

    internal class DeleteRoleCommandHandler(IAppDbContext appDbContext) : IRequestHandler<DeleteRoleCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = appDbContext.Role.FirstOrDefault(d => d.Id == request.RoleId);

            appDbContext.Role.Remove(role);
            await appDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
