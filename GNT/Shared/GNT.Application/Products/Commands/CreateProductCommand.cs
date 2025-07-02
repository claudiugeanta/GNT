using GNT.Domain.Models;
using GNT.Shared.Dtos.Products;

namespace GNT.Application.Products.Commands;

public class CreateProductCommand : IRequest<Guid>
{
    public CreateProductCommand(CreateProductDto postModel)
    {
        PostModel = postModel;
    }

    internal CreateProductDto PostModel { get; set; }
}

internal class CreateProductCommandHandler : RequestHandler<CreateProductCommand, Guid>
{
    public CreateProductCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = ProductMapping.CreateEntity(request.PostModel);

        await appDbContext.Product.AddAsync(product, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {

    }
}
