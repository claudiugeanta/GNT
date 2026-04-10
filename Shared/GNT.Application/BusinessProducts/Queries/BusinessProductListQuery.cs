using GNT.Application.Interfaces;
using GNT.Domain.Models;
using GNT.Shared.Dtos.BusinessProducts;
using GNT.Shared.Dtos.Pagination;

namespace GNT.Application.BusinessProducts.Queries;

public class BusinessProductListQuery : IRequest<PaginatedList<BusinessProductDto>>
{
    public BusinessProductListQuery(PageQuery pageQuery)
    {
        PageQuery = pageQuery;
    }

    internal PageQuery PageQuery { get; set; }
}

internal class BusinessProductListQueryHandler(IAppDbContext appDbContext, IPaginationService paginationService) : IRequestHandler<BusinessProductListQuery, PaginatedList<BusinessProductDto>>
{
    public async Task<PaginatedList<BusinessProductDto>> Handle(BusinessProductListQuery request, CancellationToken cancellationToken)
    {
        var query = appDbContext.BusinessProduct.AsQueryable();

        var page = await paginationService.PaginatedResults(query, request.PageQuery, BusinessProductMapping.DtoProjection);

        return page;
    }
}