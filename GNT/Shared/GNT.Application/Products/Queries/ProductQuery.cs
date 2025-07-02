using GNT.Domain.Models;
using GNT.Shared.Dtos.Products;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Products.Queries;

public class ProductQuery : IRequest<ProductDto>
{
    public ProductQuery(Guid id)
    {
        Id = id;
    }

    public ProductQuery(string code)
    {
        Code = code;
    }

    internal Guid? Id { get; set; }
    internal string Code { get; set; }
}

internal class ProductQueryHandler : RequestHandler<ProductQuery, ProductDto>
{
    public ProductQueryHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task<ProductDto> Handle(ProductQuery request, CancellationToken cancellationToken)
    {
        var query = appDbContext.Product.AsQueryable();

        if (request.Id.HasValue)
        {
            query = query.Where(d => d.Id == request.Id);
        }
        else if (!string.IsNullOrEmpty(request.Code))
        {
            request.Code = request.Code.ToUpper();
            query = query.Where(d => d.Code == request.Code);
        }
        else
        {
            return null;
        }

        var product = await query.Select(ProductMapping.DtoProjection)
        .FirstOrDefaultAsync(cancellationToken);

        return product;
    }
}
