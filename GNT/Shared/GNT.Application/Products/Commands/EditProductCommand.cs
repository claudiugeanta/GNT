using GNT.Shared.Errors;
using GNT.Shared.Dtos.Products;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Products.Commands;

public class EditProductCommand : IRequest<Unit>
{
    public EditProductCommand(Guid id, EditProductDto editModel)
    {
        Id = id;
        EditModel = editModel;
    }

    internal Guid Id { get; set; }
    internal EditProductDto EditModel { get; set; }
}

internal class EditProductCommandHandler : RequestHandler<EditProductCommand, Unit>
{
    public EditProductCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task<Unit> Handle(EditProductCommand request, CancellationToken cancellationToken)
    {
        var editModel = request.EditModel;

        var dbProduct = await appDbContext.Product
            .Where(d => d.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if(dbProduct == null)
        {
            throw new BusinessException(FailureCode.NotFound);
        }

        if(editModel.Name != null)
        {
            dbProduct.Name = editModel.Name;
        }

        if (editModel.Code != null)
        {
            dbProduct.Code = editModel.Code;
        }

        if (editModel.Price.HasValue)
        {
            dbProduct.Price = editModel.Price.Value;
        }

        if (editModel.Type.HasValue)
        {
            dbProduct.Type = editModel.Type.Value;
        }

        if(editModel.IsInStock.HasValue)
        {
            dbProduct.IsInStock = editModel.IsInStock.Value;
        }

        if(editModel.DatetIn.HasValue)
        {
            dbProduct.DatetIn = editModel.DatetIn.Value;
        }

        if (editModel.DateOut.HasValue)
        {
            dbProduct.DateOut = editModel.DateOut.Value;
        }

        await appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class EditProductCommandValidator : AbstractValidator<EditProductCommand>
{
    public EditProductCommandValidator()
    {

    }
}
