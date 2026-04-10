using GNT.Domain.Models;
using GNT.Shared.Dtos.BusinessProducts;
using System.Linq.Expressions;

namespace GNT.Application.Mappings;

public static class BusinessProductMapping
{
    public static Expression<Func<BusinessProduct, BusinessProductDto>> DtoProjection
    {
        get
        {
            return d => new BusinessProductDto
            {
                Id = d.Id,
                Name = d.Name,
                Price = d.Price,
                Type = d.Type,
                IsInStock = d.IsInStock,
                DatetIn = d.DatetIn,
                DateOut = d.DateOut
            };
        }
    }

    public static BusinessProduct CreateEntity(this CreateBusinessProductDto d)
    {
        return new BusinessProduct
        {
            Id = Guid.NewGuid(),
            Name = d.Name,
            Price = d.Price,
            Type = d.Type,
            IsInStock = d.IsInStock,
            DatetIn = d.DatetIn,
            DateOut = d.DateOut
        };
    }
}
