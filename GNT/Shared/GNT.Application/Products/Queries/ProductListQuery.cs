using GNT.Domain.Models;
using GNT.Shared.Dtos.Products;
using GNT.Shared.Dtos.Pagination;

namespace GNT.Application.Products.Queries;

public class ProductListQuery : IRequest<PaginatedList<ProductDto>>
{
    public ProductListQuery(PageQuery pageQuery)
    {
        PageQuery = pageQuery;
    }

    internal PageQuery PageQuery { get; set; }
}

internal class ProductListQueryHandler : RequestHandler<ProductListQuery, PaginatedList<ProductDto>>
{
    private readonly IPaginationService paginationService;
    public ProductListQueryHandler(IPaginationService paginationService, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this.paginationService = paginationService;
    }

    public override async Task<PaginatedList<ProductDto>> Handle(ProductListQuery request, CancellationToken cancellationToken)
    {
        var query = appDbContext.Product.AsQueryable();

        var page = await paginationService.PaginatedResults(query, request.PageQuery, ProductMapping.DtoProjection);

        return page;
    }
}