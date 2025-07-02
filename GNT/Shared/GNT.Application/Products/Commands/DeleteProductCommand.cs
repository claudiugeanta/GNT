using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Products.Commands;

public class DeleteProductCommand : IRequest<Unit>
{
    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }

    internal Guid Id { get; set; }
}

internal class DeleteProductCommandHandler : RequestHandler<DeleteProductCommand, Unit>
{
    public DeleteProductCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var itemToDelete = await appDbContext.Product
            .Where(d => d.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        appDbContext.Product.Remove(itemToDelete);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
