using GNT.Application.Products.Commands;
using GNT.Application.Products.Queries;
using GNT.Shared.Dtos.Products;
using GNT.Shared.Dtos.Pagination;
using GNT.Web.Server.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GNT.Web.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController : BaseController
{
    [HttpPost("get-all")]
    public async Task<PaginatedList<ProductDto>> GetAll([FromBody] PageQuery queryModel)
    {
        return await Mediator.Send(new ProductListQuery(queryModel));
    }

    [HttpGet("{id}")]
    public async Task<ProductDto> Get([FromRoute] Guid id)
    {
        return await Mediator.Send(new ProductQuery(id));
    }

    [HttpGet("{code}/code")]
    public async Task<ProductDto> GetByCode([FromRoute] string code)
    {
        return await Mediator.Send(new ProductQuery(code));
    }


    [HttpPost]
    public async Task<Guid> Create([FromBody] CreateProductDto postModel)
    {
        return await Mediator.Send(new CreateProductCommand(postModel));
    }

    [HttpPatch("{id}")]
    public async Task Edit([FromRoute] Guid id, [FromBody] EditProductDto model)
    {
        await Mediator.Send(new EditProductCommand(id, model));
    }

    [HttpDelete("{id}")]
    public async Task Delete([FromRoute] Guid id)
    {
        await Mediator.Send(new DeleteProductCommand(id));
    }
}
